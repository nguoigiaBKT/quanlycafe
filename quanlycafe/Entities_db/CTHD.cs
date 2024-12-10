namespace quanlycafe.Entities_db
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CTHD")]
    public partial class CTHD
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int maBan { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int maMon { get; set; }

        public int? soLuong { get; set; }

        public decimal? tongTien { get; set; }

        public decimal? giamGia { get; set; }

        public virtual BAN BAN { get; set; }

        public virtual MON MON { get; set; }
    }
}
