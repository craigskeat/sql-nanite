namespace SqlNaniteTest.Core30.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// The retrieve entity.
    /// </summary>
    [Table("SelectFromTable")]
    public class RetrieveEntity
    {
        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        [Key, Column("Id")]
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public string Value { get; set; }
    }
}
