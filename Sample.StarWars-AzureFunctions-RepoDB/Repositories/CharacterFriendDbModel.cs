using RepoDb.Attributes;
using StarWars.Characters;
using System;
using System.Collections.Generic;
using System.Text;

namespace StarWars.Characters.DbModels
{
    [Map("ViewStarWarsFriends")]
    public class CharacterFriendDbModel : CharacterDbModel
    {
        public int FriendOfId { get; set; }
    }
}
