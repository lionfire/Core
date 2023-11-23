namespace LionFire.FlexObjects
{
    // FUTURE: Plan for fall-through overlayed IFlex objects
    //    - IFlex will have a Flex implementation that upon a query returning not found, will search its parent, or a func<IFlex> to get a parent.  This will allow fall through, and other overlay/stacking behavior
    public interface IFlexOverlayOwner
    {
        IFlexOwner ParentFlex { get; }
    }
}

