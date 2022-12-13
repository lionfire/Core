using LionFire.Dependencies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.Referencing;

[System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
public sealed class RegisterSingletonAsInterfaceAttribute : Attribute
{
    public Type Type { get; }
    public RegisterSingletonAsInterfaceAttribute(Type type = null)
    {
        this.Type = type;
    }
}

[RegisterSingletonAsInterface]
public class ReferenceProviderService : IReferenceProviderService
{
    #region Dependencies

    IEnumerable<IReferenceProvider> referenceProviders;

    #endregion

    #region Construction

    public ReferenceProviderService(IEnumerable<IReferenceProvider> referenceProviders)
    {
        this.referenceProviders = referenceProviders;
    }

    #endregion

    public bool AllowMultipleProvidersPerScheme { get; set; } = true;

    private class ReferenceProviderServiceState
    {
        public Dictionary<string, IReferenceProvider> SchemeProviders { get; } = new Dictionary<string, IReferenceProvider>();
        public Dictionary<string, List<IReferenceProvider>> SchemeProviderLists { get; } = new Dictionary<string, List<IReferenceProvider>>();
    }

    private ReferenceProviderServiceState State
    {
        get
        {
            if (state == null)
            {
                var newState = new ReferenceProviderServiceState();

                foreach (var provider in referenceProviders)
                {
                    if (AllowMultipleProvidersPerScheme)
                    {
                        foreach (var scheme in provider.UriSchemes)
                        {
                            if (!newState.SchemeProviderLists.ContainsKey(scheme)) newState.SchemeProviderLists.Add(scheme, new List<IReferenceProvider>());
                            newState.SchemeProviderLists[scheme].Add(provider);
                        }
                    }
                    foreach (var scheme in provider.UriSchemes)
                    {
                        if (newState.SchemeProviders.ContainsKey(scheme))
                        {
                            if (!AllowMultipleProvidersPerScheme)
                            {
                                throw new AlreadyException($"{this.GetType().Name} already has an IReferenceProvider for URI scheme '{scheme}' and AllowMultipleProvidersPerScheme is set to false.");
                            }
                        }
                        else
                        {
                            newState.SchemeProviders.Add(scheme, provider);
                        }
                    }
                }
                state = newState;
            }
            return state;
        }
    }
    private ReferenceProviderServiceState state;

    public IEnumerable<Type> ReferenceTypes =>
        AllowMultipleProvidersPerScheme ?
        State.SchemeProviderLists.Values.SelectMany(list => list).SelectMany(provider => provider.ReferenceTypes).Distinct() :
        State.SchemeProviders.Values.SelectMany(provider => provider.ReferenceTypes).Distinct();

    public IEnumerable<string> UriSchemes =>
        AllowMultipleProvidersPerScheme ?
        (IEnumerable<string>)State.SchemeProviderLists.Keys :
        State.SchemeProviders.Keys;

    //public IReferenceProvider TryGetReferenceProvider(string uri)
    //{
    //    var scheme = LionUri.TryGetUriScheme(uri);
    //    if (scheme == null) return null;
    //    if (AllowMultipleProvidersPerScheme)
    //    {
    //        if (State.SchemeProviderLists.TryGetValue(scheme, out var providers))
    //        {
    //            foreach (var provider in

    //        }
    //        throw new NotImplementedException(nameof(AllowMultipleProvidersPerScheme));
    //    }
    //    else
    //    {
    //        if (State.SchemeProviders.TryGetValue(scheme, out IReferenceProvider provider))
    //        {
    //            return provider;
    //        }
    //    }
    //    return null;
    //}

    public IEnumerable<IReferenceProvider> TryGetReferenceProviders(string uri)
    {
        var scheme = LionUri.TryGetUriScheme(uri);
        if (scheme != null && State.SchemeProviderLists.TryGetValue(scheme, out var providers))
        {
            return providers;
        }
        return Enumerable.Empty<IReferenceProvider>();
    }

    //public IReferenceProvider GetReferenceProvider(string uri)
    //{
    //    var result = TryGetReferenceProvider(uri);
    //    if (result != null) return result;

    //    var scheme = LionUri.TryGetUriScheme(uri);
    //    if (scheme == null) throw new ArgumentException($"Specified {nameof(uri)} is missing a URI scheme.");
    //    throw new KeyNotFoundException();
    //}

    public (TReference result, string error) TryGetReference<TReference>(string uri, bool aggregateErrors = false) where TReference : IReference
    {
        StringBuilder? sb = null;

        foreach (var provider in TryGetReferenceProviders(uri))
        {
            var result = provider.TryGetReference<TReference>(uri);
            if (result.result != null) { return result; }
            else if (aggregateErrors)
            {
                sb ??= new StringBuilder();
                sb.AppendLine($"{provider.GetType().Name}: {result.error ?? "(No error given)"}");
            }
        }
        return (default, sb?.ToString() ?? "(No error available)");
    }
    //public TReference GetReference<TReference>(string uri) where TReference : IReference => GetReferenceProvider(uri).GetReference<TReference>(uri);
}
