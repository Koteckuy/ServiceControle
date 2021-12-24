namespace SCS
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Device")]
    public partial class Device
    {
        public int ID { get; set; }

        public int ClientID { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(50)]
        public string Brand { get; set; }

        [StringLength(50)]
        public string Model { get; set; }

        [StringLength(12)]
        public string SerialNumber { get; set; }

        [StringLength(100)]
        public string Defect { get; set; }

        public bool Deleted { get; set; }

        public virtual Client Client { get; set; }
    }
}
