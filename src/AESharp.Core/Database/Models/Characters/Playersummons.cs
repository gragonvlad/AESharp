// This file was automatically generated

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AESharp.Core.Database.Models.Characters
{
    public sealed class Playersummons
    {
            [Key, Column( "ownerguid" )]
            public uint Ownerguid { get; set; }

            [Column( "entry" )]
            public uint Entry { get; set; }

            [Column( "name" ), Required]
            public string Name { get; set; }

    }
}