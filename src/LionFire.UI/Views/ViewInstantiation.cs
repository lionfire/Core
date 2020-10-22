namespace LionFire.Shell
{
    public class ViewInstantiation : ViewReference
    {
        /// <summary>
        /// (TODO) Null: auto-generate presenter names, starting with MainPresenter
        /// </summary>
        public string PresenterName { get; set; }

        public string ViewName { get; set; }


        public bool KeepsApplicationAlive { get; set; }
    }
}
