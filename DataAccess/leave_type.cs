//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataAccess
{
    using System;
    using System.Collections.Generic;
    
    public partial class leave_type
    {
        public leave_type()
        {
            this.leavs = new HashSet<leav>();
        }
    
        public string jobCategory { get; set; }
        public string leaveCategory { get; set; }
        public int maxAllowed { get; set; }
    
        internal virtual ICollection<leav> leavs { get; set; }
    }
}
