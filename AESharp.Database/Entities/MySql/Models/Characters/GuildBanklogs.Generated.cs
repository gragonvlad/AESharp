// This file was automatically generated

using System.ComponentModel.DataAnnotations.Schema;

namespace AESharp.Database.Entities.MySql.Models.Characters
{
    public sealed class GuildBankLogs
    {
            [Column( "log_id" )]
            public int LogId { get; set; }

            [Column( "guildid" )]
            public int Guildid { get; set; }

            // tab 6 is money logs
            [Column( "tabid" )]
            public int Tabid { get; set; }

            [Column( "action" )]
            public int Action { get; set; }

            [Column( "player_guid" )]
            public int PlayerGuid { get; set; }

            [Column( "item_entry" )]
            public int ItemEntry { get; set; }

            [Column( "stack_count" )]
            public int StackCount { get; set; }

            [Column( "timestamp" )]
            public int Timestamp { get; set; }

    }
}