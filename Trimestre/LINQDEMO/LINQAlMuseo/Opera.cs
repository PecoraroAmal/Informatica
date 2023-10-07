﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LINQAlMuseo;

public class Opera
{
    public int Id { get; set; }
    public string Titolo { get; set; } = null!;
    public decimal Quotazione { get; set; }
    public int FkArtista { get; set; }
    public override string ToString()
    {
        return String.Format($"[ID = {Id}, Titolo = {Titolo}, Quotazione = {Quotazione},  FkArtista = {FkArtista}]"); ;
    }
}
