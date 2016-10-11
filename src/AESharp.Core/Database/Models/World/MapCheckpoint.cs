// This file was automatically generated

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AESharp.Core.Database.Models.World
{
    public sealed class MapCheckpoint
    {
            [Key, Column( "entry" )]
            public int Entry { get; set; }

            [Column( "prereq_checkpoint_id" )]
            public int PrereqCheckpointId { get; set; }

            [Column( "creature_id" )]
            public int CreatureId { get; set; }

            [Column( "name" ), Required]
            public string Name { get; set; }

    }
}