//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataAccess
{
    using System;
    using System.Collections.Generic;
    
    public partial class task
    {
        public int TID { get; set; }
        public System.DateTime date { get; set; }
        public System.TimeSpan time { get; set; }
        public string activity { get; set; }
        public string status { get; set; }
        public string EID { get; set; }
    
        internal virtual employee employee { get; set; }
    }
}
