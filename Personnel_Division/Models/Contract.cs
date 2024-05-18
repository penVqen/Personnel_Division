﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Personnel_Division.Models;

[Table("Contract")]
public partial class Contract
{
    [Key]
    public int ID_Contract { get; set; }

    [Required]
    [StringLength(20)]
    [Unicode(false)]
    public string Type { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string Validity_period { get; set; }

    [Required]
    [StringLength(50)]
    [Unicode(false)]
    public string Date_conclusion { get; set; }

    public int? ID_Worker { get; set; }

    [ForeignKey("ID_Worker")]
    [InverseProperty("Contracts")]
    public virtual Worker ID_WorkerNavigation { get; set; }
}