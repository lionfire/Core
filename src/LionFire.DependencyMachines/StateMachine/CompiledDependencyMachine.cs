using LionFire.ExtensionMethods;
using LionFire.ExtensionMethods.Collections;
using LionFire.Ontology;
using LionFire.Structures;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace LionFire.DependencyMachines
{
    public class CompiledDependencyMachine
    {
        public IServiceProvider ServiceProvider { get; }
        #region Parameters

        public DependencyMachineConfig Config { get; }

        public IEnumerable<IParticipant> Participants => participants;
        private readonly IEnumerable<IParticipant> participants;

        #endregion

        #region Derived

        public IEnumerable<DependencyStage> Stages => stages;

        readonly List<DependencyStage> stages = new List<DependencyStage>();

        public IEnumerable<DependencyStage> DisabledStages => disabledStages;

        readonly List<DependencyStage> disabledStages = new List<DependencyStage>();

        public IEnumerable<IParticipant> DisabledParticipants => disabledStages.SelectMany(s => s.Members);

        // REVIEW - public properties for these?
        Dictionary<string, IParticipant> providedObjects = new Dictionary<string, IParticipant>();
        Dictionary<string, List<IParticipant>> contributedObjects = new Dictionary<string, List<IParticipant>>();
        Dictionary<string, List<IParticipant>> dependencyDict = new Dictionary<string, List<IParticipant>>();

        #endregion

        #region Construction

        public CompiledDependencyMachine(IServiceProvider serviceProvider, DependencyMachineConfig config)
        {
            ServiceProvider = serviceProvider;
            Config = config;

            participants = Config.ParticipantInstances
                .Concat(CreateInjectedParticipants())
               .Concat(AutoRegisteredFromTypes())
                .ToList();

            Calculate();
        }

        #endregion

        #region Parameters

        public bool TryToKeepContributorsTogether { get; set; } = false; // FIXME: true causes infinite loop for vos -- is Fallback not working?
        public bool FallbackToEarlyContributorsIfNeeded { get; set; } = true; // Only used if TryToKeepContributorsTogether is true  

        #endregion

        #region (Private) Logic

        protected IEnumerable<IParticipant> CreateInjectedParticipants()
            => Config.InjectedParticipants.Select(ip => ip(ServiceProvider));
        protected IEnumerable<IParticipant> AutoRegisteredFromTypes()
            => Config.AutoRegisterFromServiceTypes.SelectMany(t => (ServiceProvider.GetService(t) as IHasMany<IParticipant>)?.Objects ?? Enumerable.Empty<IParticipant>());

        private static string agg(string x, string y) => $"{x}, {y}";
        private void Calculate()
        {
            var unsolved = new Dictionary<string, IParticipant>();
            unsolved.TryAddRange(participants.Select(p => new KeyValuePair<string, IParticipant>(p.Key, p)));

            Dictionary<string, List<IParticipant>> contributorsRemaining = new Dictionary<string, List<IParticipant>>();

            #region Gather lists of: provided, contributes, dependencies, beforeRequirements

            Dictionary<string, List<string>> beforeRequirements = new Dictionary<string, List<string>>();
            foreach (var member in participants)
            {
                var provides = member.EffectiveProvides();
                if (provides.Any())
                {
                    foreach (var provided in provides.Select(p => p.ToDependencyKey()))
                    {
                        if (providedObjects.ContainsKey(provided))
                        {
                            throw new AlreadyException($"'{provided}' is already provided by another member.  If there are multiple instances provided, use contributes instead of provides.");
                        }
                        providedObjects.Add(provided, member);
                    }
                }

                if (member.Contributes != null)
                {
                    foreach (var contributed in member.Contributes.Select(p => p.ToDependencyKey()))
                    {
                        if (providedObjects.ContainsKey(contributed))
                        {
                            throw new AlreadyException($"'{contributed}' cannot be both Provided by one IParticipant and Contributed by another.  If multiple participants contribute it, use only Contributed.");
                        }
                        contributedObjects.GetOrAdd(contributed).Add(member);
                        contributorsRemaining.GetOrAdd(contributed).Add(member);
                    }
                }

                if (member.Dependencies != null)
                {
                    foreach (var dependency in member.Dependencies.Select(p => p?.ToDependencyKey()).Where(d => d != null))
                    {
                        dependencyDict.GetOrAdd(dependency).Add(member);
                    }
                }

                if (member.PrerequisiteFor != null)
                {
                    foreach (var dependant in member.PrerequisiteFor.Select(p => p.ToDependencyKey()))
                    {
                        var placeholderKey = $"{dependant} placeholder";
                        if (!unsolved.ContainsKey(placeholderKey))
                        {
                            unsolved.Add(new Placeholder(placeholderKey));
                        }
                        beforeRequirements.GetOrAdd(dependant).Add(member.Key);
                    }
                }
            }

            #endregion



            var solved = new List<IParticipant>();
            var availableDependencies = new HashSet<string>();
            var availableDependencyHandles = new HashSet<string>();

            int lastUnsolvedCount = -1;
            HashSet<string> lastUnsolvedDependencies = new HashSet<string>();
            HashSet<string> lastUnsolvedAfters = new HashSet<string>();
            HashSet<string> lastUnsolvedDependencyHandles = new HashSet<string>();

            var potentiallyContributedKeys = new Dictionary<string, List<IParticipant>>();
            bool allowEarlyContributors = !TryToKeepContributorsTogether; //False: Try to batch those who contribute something together


            bool tryingAgain = false;
        tryAgain:
            for (; tryingAgain || lastUnsolvedCount != unsolved.Count && unsolved.Count > 0;)
            {
                DependencyStage stage;
                lastUnsolvedDependencies.Clear();
                lastUnsolvedAfters.Clear();
                lastUnsolvedDependencyHandles.Clear();
                lastUnsolvedCount = unsolved.Count;
                potentiallyContributedKeys.Clear();

                stage = new DependencyStage() { AllowNoDependencies = true };

                //var cannotContributeYet = new HashSet<string>();
                var contributionHasUnsolvedMembers = new HashSet<string>();

                foreach (var member in unsolved.Values.ToArray())
                {
                    #region Skip if beforeRequirements not satisfied

                    if (beforeRequirements.ContainsKey(member.Key))
                    {
                        var unfulfilledBeforePrerequisites = beforeRequirements[member.Key]
                            .Intersect(
                                unsolved.Select(u => u.Key)
                                    //.Concat(unsolved.SelectMany(u => u.Contributes ?? Enumerable.Empty<object?>()))
                                    //.Select(u => u.KeyForContributed())
                                    , EqualityComparer<string>.Default);
                        if (unfulfilledBeforePrerequisites.Any())
                        {
                            Debug.WriteLine($"{member.Key} has unfulfilled inbound prerequisites: {unfulfilledBeforePrerequisites.Aggregate((x, y) => $"{x}, {y}")}");
                            lastUnsolvedDependencies.AddRange(unfulfilledBeforePrerequisites);
                            StillUnsolved(member);
                            continue;
                        }
                    }

                    #endregion

                    #region Skip if After requirements are not satisfied

                    if (member.After != null)
                    {
                        bool aftersAreMissing = false;

                        foreach (var missing in member.After.Where(d => !availableDependencies.Contains(d.ToDependencyKey())))
                        {
                            var key = missing.ToDependencyKey();
                            lastUnsolvedAfters.Add(key);
                            if (providedObjects.ContainsKey(key) || contributedObjects.ContainsKey(key)) aftersAreMissing = true;
                            else
                            {
                                Trace.WriteLine("After is never provided.  Ignoring requirement to be After: " + key);
                            }
                        }
                        if (aftersAreMissing)
                        {
                            Debug.WriteLine($"[unsolved] '{member.Key}' must be after '{member.After.Select(a => a.ToDependencyKey()).Where(d => !availableDependencies.Contains(d)).Aggregate(agg)}'");
                            StillUnsolved(member);
                            continue;
                        }
                    }

                    #endregion

                    #region Skip if Dependencies are not satisfied
                    {
                        var missingDependencies = member.Dependencies?.Select(d => d?.ToDependencyKey())
                            .Where(d => d != null /* null dependency means it's explicitly ok to be first to initialize */
                                && !availableDependencies.Contains(d));
                        if (missingDependencies?.Any() == true)
                        {
                            lastUnsolvedDependencies.AddRange(missingDependencies);
                            Debug.WriteLine($"[unsolved] '{member.Key}' depends on '{missingDependencies.Aggregate(agg)}'");
                            StillUnsolved(member);
                            continue;
                        }
                    }
                    #endregion

                    #region Skip if DependencyHandles are not satisfied
                    {
                        var missingDependencyHandles = member.DependencyHandles?.Select(d => d.Key.ToDependencyKey())
                            .Where(h => !availableDependencyHandles.Contains(h));
                        if (missingDependencyHandles?.Any() == true)
                        {
                            lastUnsolvedDependencyHandles.AddRange(missingDependencyHandles);
                            Debug.WriteLine($"[unsolved] '{member.Key}' depends on '{missingDependencyHandles.Aggregate(agg)}'");
                            StillUnsolved(member);
                            continue;
                        }
                    }
                    #endregion

                    #region Special case for Contributors -- handled as solved or unsolved later

                    if (member.Contributes != null && member.Contributes.Any())
                    {
                        #region Skip if saving a set of contributions to the same contribution key for later

                        if (!allowEarlyContributors && member.Contributes.Where(c => contributionHasUnsolvedMembers.Contains(c)).Any()) continue;

                        #endregion

                        #region Maybe mark this as solved in the next step

                        foreach (var contribution in member.Contributes)
                        {
                            potentiallyContributedKeys.GetOrAdd(contribution.ToDependencyKey()).Add(member);
                        }
                        continue;

                        #endregion
                    }

                    #endregion

                    OnSolved(stage, member);

                }
                void OnSolved(DependencyStage stage, IParticipant solved)
                {
                    stage.Add(solved);

                    unsolved.Remove(solved.Key);
                    Debug.WriteLine($"[SOLVED] '{solved.Key}'");

                    if (solved.Contributes != null)
                    {
                        foreach (var contributed in solved.Contributes)
                        {
                            var contributedKey = contributed.ToDependencyKey();
                            if (!contributorsRemaining.ContainsKey(contributedKey))
                            {
                                Debug.WriteLine("Unexpected error: !contributors.ContainsKey(contributedKey)"); // TOASSERT
                                continue;
                            }

                            var contributorsRemainingForKey = contributorsRemaining[contributedKey];
                            if (contributorsRemainingForKey.Remove(solved))
                            {
                                if (contributorsRemainingForKey.Count == 0)
                                {
                                    // Last remaining contributor was solved.  Count this as a provided dependency.
                                    availableDependencies.Add(contributedKey);
                                }
                            }
                            else
                            {
                                Debug.WriteLine("Unexpected: Contributors - !Remove"); // TOASSERT
                            }
                        }
                    }
                }

                void StillUnsolved(IParticipant member)
                {
                    if (member.Contributes != null)
                    {
                        foreach (var contribution in member.Contributes)
                        {
                            contributionHasUnsolvedMembers.Add(contribution.ToDependencyKey());

                            //if (!allowEarlyContributors && potentiallyContributedKeys.ContainsKey(contribution.ToDependencyKey()))
                            //{
                            //    //potentialContributors.Remove(contribution.ToDependencyKey());

                            //    foreach (var potentiallyContributingMember in potentiallyContributedKeys[contribution.ToDependencyKey()])
                            //    {
                            //        //if (contributionHasUnsolvedMembers.Contains(kvp.Key))
                            //        //{
                            //        //foreach (var potentiallyContributingMember in kvp.Value)
                            //        //{
                            //        Debug.WriteLine($"[unsolved] Early contribution is not allowed, so '{potentiallyContributingMember.Key}' cannot contribute '{contribution.ToDependencyKey()}' yet");
                            //        StillUnsolved(member);
                            //        //}
                            //        //continue;
                            //        //}
                            //    }
                            //}
                            //potentiallyContributedKeys.Remove(contribution.ToDependencyKey());
                            //cannotContributeYet.Add(contribution.ToDependencyKey());
                        }
                    }
                }

                #region Completed Contributions: register as available

                foreach (var contributionMembers in potentiallyContributedKeys)
                {
                    if (contributionHasUnsolvedMembers.Contains(contributionMembers.Key))
                    {
                        if (!allowEarlyContributors)
                        {
                            foreach (var member in contributionMembers.Value)
                            {
                                Debug.WriteLine($"[unsolved] Early contribution is not allowed, so '{member.Key}' cannot contribute '{contributionMembers.Key}' yet");
                                StillUnsolved(member);
                            }
                        }
                        continue;
                    }
                    foreach (var member in contributionMembers.Value)
                    {
                        OnSolved(stage, member);
                    }
                    availableDependencies.Add(contributionMembers.Key);
                }

                #endregion

                if (!stage.IsUseless)
                {
                    stage.Id = stages.Count;
                    stages.Add(stage);

                    availableDependencies.AddRange(stage.Provides?.Select(p => p.ToDependencyKey()) ?? Enumerable.Empty<string>());
                    //availableDependencies.Add(stage.Key); // REVIEW - add keys?
                }

                // ENH: If stage has a DependencyStagePlaceholder, use that as the Name for the stage.
            }

            if (unsolved.Count > 0 && TryToKeepContributorsTogether && FallbackToEarlyContributorsIfNeeded && !tryingAgain)
            {
                tryingAgain = true;
                goto tryAgain;
            }

            foreach (var stage in stages.ToArray())
            {
                //var dependencies = new HashSet<object>();
                //foreach (var member in stage.Members)
                //{
                //    dependencies.TryAddRange(member.Dependencies);
                //}
                //stage.Dependencies = dependencies;

                if (!stage.Dependencies.Any() && !stage.AllowNoDependencies)
                {
                    switch (Config.OrphanedStagePolicy)
                    {
                        case OrphanedStagePolicy.Throw:
                            throw new ArgumentException($"Stage {stage.ToLongString()} is an orphan because it has no dependencies and {typeof(DependencyMachineConfig).Name}.{nameof(DependencyMachineConfig.OrphanedStagePolicy)} is set to {nameof(OrphanedStagePolicy.Throw)}");
                        case OrphanedStagePolicy.Disable:
                            disabledStages.Add(stage);
                            stages.Remove(stage);
                            break;
                        case OrphanedStagePolicy.Execute:
                            // Do nothing
                            break;
                        default:
                            throw new ArgumentException($"Unknown {nameof(OrphanedStagePolicy)}: {Config.OrphanedStagePolicy}");
                    }
                }
            }
            if (unsolved.Count > 0)
            {
                throw new DependenciesUnresolvableException("Unable to solve dependencies: " +
                    lastUnsolvedDependencies.Concat(lastUnsolvedDependencyHandles).Aggregate((x, y) => $"{x}, {y}"))
                {
                    UnresolvableDependencies = lastUnsolvedDependencies.Concat(lastUnsolvedDependencyHandles)
                };
            }
        }

        #endregion
    }
}
