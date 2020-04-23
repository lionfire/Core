using LionFire.ExtensionMethods;
using LionFire.Structures;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LionFire.DependencyMachines
{
    public class CompiledDependencyMachine
    {
        public DependenyMachineDefinition Definition { get; }

        public IEnumerable<DependencyStage> Stages => stages;
        List<DependencyStage> stages = new List<DependencyStage>();
        Dictionary<object, IParticipant> providedObjects = new Dictionary<object, IParticipant>();
        Dictionary<string, List<IParticipant>> contributedObjects = new Dictionary<string, List<IParticipant>>();
        Dictionary<string, List<IParticipant>> dependencyDict = new Dictionary<string, List<IParticipant>>();

        public CompiledDependencyMachine(DependenyMachineDefinition definition)
        {
            Definition = definition;
            Calculate();
        }

        private void Calculate()
        {
            foreach (var member in Definition.Participants)
            {
                foreach (var provided in member.Provides)
                {
                    if (providedObjects.ContainsKey(provided))
                    {
                        throw new AlreadyException($"'{provided}' is already provided by another member.  If there are multiple instances provided, use contributes instead of provides.");
                    }
                    providedObjects.Add(provided, member);
                }

                foreach (var contributed in member.Contributes)
                {
                    contributedObjects.GetOrAdd(contributed.KeyForContributed()).Add(member);
                }

                foreach (var dependency in member.Dependencies)
                {
                    dependencyDict.GetOrAdd(dependency.KeyForContributed()).Add(member);
                }
            }

            var unsolved = new List<IParticipant>(Definition.Participants);
            var solved = new List<IParticipant>();
            int lastUnsolvedCount = -1;
            var availableDependencies = new HashSet<string>();
            var availableDependencyHandles = new HashSet<string>();

            DependencyStage stage;
            HashSet<string> lastUnsolvedDependencies = new HashSet<string>();
            HashSet<string> lastUnsolvedDependencyHandles = new HashSet<string>();

            for (; lastUnsolvedCount != unsolved.Count && unsolved.Count > 0;)
            {
                lastUnsolvedDependencies.Clear();
                lastUnsolvedDependencyHandles.Clear();
                lastUnsolvedCount = unsolved.Count;

                var potentialContributors = new Dictionary<string, List<IParticipant>>();

                stage = new DependencyStage();

                var cannotContributeYet = new HashSet<string>();

                foreach (var member in unsolved.ToArray())
                {
                    if (member.Dependencies.Where(d => !availableDependencies.Contains(d.KeyForContributed())).Any()
                        || member.DependencyHandles.Where(h => !availableDependencyHandles.Contains(h.Key)).Any()
                        )
                    {
                        foreach (var contribution in member.Contributes)
                        {
                            cannotContributeYet.Add(contribution.KeyForContributed());
                            potentialContributors.Remove(contribution.KeyForContributed());
                        }
                        foreach (var missing in member.Dependencies?.Where(d => !availableDependencies.Contains(d.KeyForContributed())))
                        {
                            lastUnsolvedDependencies.Add(missing.KeyForContributed());
                        }
                        foreach (var missing in member.DependencyHandles?.Where(h => !availableDependencyHandles.Contains(h.Key)))
                        {
                            lastUnsolvedDependencyHandles.Add(missing.KeyForContributed());
                        }
                        continue;
                    }

                    if (member.Contributes != null && member.Contributes.Any())
                    {
                        if (member.Contributes.Where(c => cannotContributeYet.Contains(c)).Any()) continue;

                        foreach (var contribution in member.Contributes)
                        {
                            potentialContributors.GetOrAdd(contribution.KeyForContributed()).Add(member);
                        }
                        continue;
                    }
                    stage.Add(member);
                    availableDependencies.Add(member.Key);
                    foreach(var dependency in member.Provides)
                    {
                        availableDependencies.Add(dependency.KeyForContributed());
                    }
                    unsolved.Remove(member);
                }

                foreach (var kvp in potentialContributors)
                {
                    var potentiallyContributed = kvp.Key;
                    foreach (var member in kvp.Value)
                    {
                        stage.Add(member);
                        unsolved.Remove(member);
                        availableDependencies.Add(member.Key);
                    }
                    availableDependencies.Add(kvp.Key);
                }

                stage.Id = stages.Count;
                stages.Add(stage);

                // ENH: If stage has a DependencyStagePlaceholder, use that as the Name for the stage.
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
    }
}
