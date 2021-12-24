namespace SCS
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Master")]
    public partial class Master
    {
        public int ID { get; set; }

        [Required]
        [StringLength(50)]
        public string FName { get; set; }

        [Required]
        [StringLength(50)]
        public string SName { get; set; }

        [StringLength(50)]
        public string TName { get; set; }

        [Required]
        [StringLength(12)]
        public string Phone { get; set; }

        [StringLength(100)]
        public string Address { get; set; }

        public bool Deleted { get; set; }
    }
}
