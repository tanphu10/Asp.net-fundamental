﻿namespace DemoApi.Data.ViewModels
{
    public class FunctionActionViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ParentId { get; set; }
        public bool? HasCreate { get; set; }
        public bool? HasUpdate { get; set; }
        public bool? HasDelete { get; set; }
        public bool? HasView { get; set; }
        public bool? HasImport { get; set; }
        public bool? HasExport { get; set; }
        public bool? HasApprove { get; set; }
    }
}
