using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LionFire.Info.HierarchicalTags;
using LionFire.Structures;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace LionFire.Info
{
    public partial class HierarchicalTagEditor
    {

        [Parameter]
        public string? TagContextName { get; set; }
        public HierarchicalTagContext? TagContext { get; set; }

        public TagNode? SelectedValue { get; set; }

        bool loaded = false;
        protected override async Task OnInitializedAsync()
        {
            TagContext = await NS.GetOrCreateAsync(TagContextName ?? throw new ArgumentNullException(nameof(TagContextName)));

            TagContext.Name = TagContextName;

            if (TagContext.Children == null || TagContext.Children.Count == 0) { 
            var root1 = new TagNode(null, "root1");
            TagContext.Add(root1);
            var a1 = new TagNode(root1, "a1");
            root1.Add(a1);
            var a1_1 = new TagNode(a1, "a1.1");
            a1.Add(a1_1);
            var a1_2 = new TagNode(a1, "a1.2");
            a1.Add(a1_2);
            var a2 = new TagNode(root1, "a2");
            root1.Add(a2);
            var root2 = new TagNode(null, "root2");
            TagContext.Add(root2);
        }


            await base.OnInitializedAsync();
        }


        private void ShowAddDialog()
        {
            var options = new DialogOptions { CloseOnEscapeKey = true, CloseButton = true  };
            DialogService.Show<AddTagDialog>($"Add child to {SelectedValue}", options);
        }
    }
}