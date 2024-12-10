namespace quanlycafe.Entities_db
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MON")]
    public partial class MON
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MON()
        {
            CTHD = new HashSet<CTHD>();
        }

        [Key]
        public int maMon { get; set; }

        [StringLength(30)]
        public string tenMon { get; set; }

        public decimal? giaBan { get; set; }

        public int? maDanhMuc { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CTHD> CTHD { get; set; }

        public virtual DANHMUC DANHMUC { get; set; }
    }
}
