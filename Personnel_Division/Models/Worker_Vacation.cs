﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Personnel_Division.Models;

[Table("Worker_Vacation")]
public partial class Worker_Vacation
{
    [Key]
    public int ID_Worker_Vacation { get; set; }

    public int ID_Worker { get; set; }

    public int ID_Vacation { get; set; }

    [ForeignKey("ID_Vacation")]
    [InverseProperty("Worker_Vacations")]
    public virtual Vacation ID_VacationNavigation { get; set; }

    [ForeignKey("ID_Worker")]
    [InverseProperty("Worker_Vacations")]
    public virtual Worker ID_WorkerNavigation { get; set; }
}