﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Personnel_Division.Models;

[Table("Vacancy")]
public partial class Vacancy
{
    [Key]
    public int ID_Vacancy { get; set; }

    public int ID_Division { get; set; }

    [Required]
    [StringLength(50)]
    [Unicode(false)]
    public string Title { get; set; }

    [Column(TypeName = "date")]
    public DateTime Start_Date_accepting_applications { get; set; }

    [Column(TypeName = "date")]
    public DateTime End_date_accepting_applications { get; set; }

    [ForeignKey("ID_Division")]
    [InverseProperty("Vacancies")]
    public virtual Division ID_DivisionNavigation { get; set; }

    [InverseProperty("ID_VacancyNavigation")]
    public virtual ICollection<Worker_Vacancy> Worker_Vacancies { get; set; } = new List<Worker_Vacancy>();
}