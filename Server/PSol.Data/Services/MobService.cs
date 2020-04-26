using Bindings;
using DMod.Models;
using Data.Repositories.Interfaces;
using Data.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

namespace Data.Services
{
    public class MobService : IMobService
    {
        private static readonly Random rnd = new Random();
        private readonly IMobRepository _mobRepo;
        private readonly IUserService _userService;
        private readonly ICollection<Mob> _mobs;
        private List<Combat> _pendingCombats;
        private List<Combat> _readyCombats;
        private const int CombatDistance = 2000;

        public MobService(IMobRepository mobRepo, IUserService userService)
        {
            _mobRepo = mobRepo;
            _userService = userService;
            _mobs = _mobRepo.GetAllMobs();
        }

        public ICollection<Mob> GetMobs(int minX = 0, int maxX = Constants.PLAY_AREA_WIDTH, int minY = 0, int maxY = Constants.PLAY_AREA_HEIGHT, bool getDead = false)
        {
            return getDead ? _mobs.Where(m => m.X >= minX && m.X <= maxX && m.Y >= minY && m.Y <= maxY).ToList()
                : _mobs.Where(m => m.X >= minX && m.X <= maxX && m.Y >= minY && m.Y <= maxY && m.Alive).ToList();
        }

        public void RepopGalaxy(bool forceAll = false)
        {
            Cnsl.Log("Server Repop", true);
            // Get all mobs and count them
            var activeMobs = GetMobs(getDead: true);
            var countOfAllMobs = activeMobs.GroupBy(g => g.MobTypeId)
                .Select(s => new Tuple<string, int>(s.Key, s.Count())).ToList();
            // Get all mob types
            var mobTypes = _mobRepo.GetAllMobTypes();
            // Go through all mob types and see if any are missing and add them if so
            foreach (var mobType in mobTypes)
            {
                var tuple = countOfAllMobs.Find(a => a.Item1 == mobType.Id);
                // Found a tuple so at least some of the mobs exists
                if (tuple != null)
                {
                    // If the number that exist match the type's max spawned allowed, skip this type
                    if (tuple.Item2 >= mobType.MaxSpawned) continue;
                    // Not enough are out there so add more
                    for (var i = tuple.Item2; i != mobType.MaxSpawned; i++)
                    {
                        AddDeadMob(mobType);
                    }
                }
                else
                {
                    // Tuple wasn't found so add all mobs as dead so they will spawn
                    for (var i = 0; i != mobType.MaxSpawned; i++)
                    {
                        AddDeadMob(mobType);
                    }
                }
            }

            // Now we know that all of the possible mobs exist in the db (dead or alive). Look at the
            // dead ones and see if any are able to be spawned.
            var deadMobs = activeMobs.Where(m => m.Alive == false);
            // Go through all the dead mobs and check their spawn timer against their death time
            foreach (var mob in deadMobs)
            {
                // If difference is less than min spawn time, skip them
                if (DateTime.UtcNow.Subtract(mob.KilledDate).CompareTo(new TimeSpan(0, 0, mob.MobType.SpawnTimeMin)) < 0) continue;

                // If difference is between min and max, flip a coin and see if they want to spawn
                var coin = rnd.Next(0, 2) == 0;
                if (DateTime.UtcNow.Subtract(mob.KilledDate).CompareTo(new TimeSpan(0, 0, mob.MobType.SpawnTimeMax)) < 0 && coin) continue;

                // Otherwise the coin flip passed or we've passed max spawn time
                // Determine if he's going to be special
                mob.Special = rnd.Next(0, 10) == 7;

                mob.Alive = true;
                mob.Health = mob.MobType.MaxHealth;
                mob.Rotation = rnd.Next(0, 360);
                mob.Shield = mob.MobType.MaxShield;
                mob.SpawnDate = DateTime.UtcNow;
                var xMod = rnd.Next(-1 * mob.MobType.SpawnRadius, mob.MobType.SpawnRadius);
                var yMod = rnd.Next(-1 * mob.MobType.SpawnRadius, mob.MobType.SpawnRadius);
                mob.X = mob.MobType.Star.X + xMod;
                mob.X = mob.X < 0 ? mob.X * -1 : mob.X;
                mob.Y = mob.MobType.Star.Y + yMod;
                mob.Y = mob.Y < 0 ? mob.Y * -1 : mob.Y;
                mob.Name = GenerateName(mob.Special);
            }
            Cnsl.Finalize("Server Repop", true);
        }

        private void AddDeadMob(MobType type)
        {
            var mob = new Mob
            {
                Id = Guid.NewGuid().ToString(),
                Alive = false,
                KilledDate = DateTime.UtcNow,
                MobTypeId = type.Id,
                MobType = type,
                Special = false
            };
            _mobs.Add(mob);
        }

        public void SaveMobs()
        {
            if (_mobs == null) return;
            foreach (var mob in _mobs)
            {
                _mobRepo.SaveMob(mob);
            }
        }

        public void WanderMobs()
        {
            // Go through mobs and wander them a little within their spawn radius
            GetMobs().ToList().ForEach(m =>
            {
                // 0.2% chance to wander
                if (rnd.Next(0, 999) > 100) return;
                var negX = rnd.Next(0, 2);
                var negY = rnd.Next(0, 2);
                var newX = m.X + rnd.Next(200, 500) * (negX == 0 ? 1f : -1f);
                var newY = m.Y + rnd.Next(200, 500) * (negY == 0 ? 1f : -1f);
                Cnsl.Log("NewX: " + newX + " : NewY: " + newY);

                // Make sure they don't wander out of their zone
                if (newX > m.MobType.Star.X + 2 * m.MobType.SpawnRadius)
                {
                    newX = m.MobType.Star.X + 2 * m.MobType.SpawnRadius;
                }
                else if (newX < m.MobType.Star.X - 2 * m.MobType.SpawnRadius)
                {
                    newX = m.MobType.Star.X - 2 * m.MobType.SpawnRadius;
                }
                if (newY > m.MobType.Star.Y + 2 * m.MobType.SpawnRadius)
                {
                    newY = m.MobType.Star.Y + 2 * m.MobType.SpawnRadius;
                }
                else if (newY < m.MobType.Star.Y - 2 * m.MobType.SpawnRadius)
                {
                    newY = m.MobType.Star.Y - 2 * m.MobType.SpawnRadius;
                }

                m.X = newX;
                m.Y = newY;
            });


            // Let the UI handle this
            // Actually wander them
            /*GetMobs().Where(m => m.NavToX != null).ToList().ForEach(m =>
            {
                var start = new Vector2(m.X, m.Y);
                var destination = new Vector2(m.NavToX ?? m.X - 0.1f, m.NavToY ?? m.Y - 0.1f);
                var direction = Vector2.Normalize(destination - start);
                var distance = Vector2.Distance(start, destination);
                m.Rotation = (float)Math.Atan2(direction.Y, direction.X) + MathHelper.ToRadians(90);

                m.X += direction.X * 4f;
                m.Y += direction.Y * 4f;
                if (distance.CompareTo(50F) > 0) return;
                m.NavToX = null;
                m.NavToY = null;
            });*/
        }

        public void CheckAggro()
        {
            // Get mobs not in combat and see if any players are close enough to agro
            GetMobs().Where(m => m.TargettingId == null).ToList().ForEach(CheckSingleAggro);
        }

        private void CheckSingleAggro(Mob m)
        {
            m.TargettingId = null;
            var players = _userService.ActiveUsers.Where(p => p.X > (m.X - 100) && p.X < (m.X + 100) && p.Z > (m.Y - 100) && p.Z < (m.Y + 100)).ToList();
            if (players.Count == 0) return;
            var ndx = 0;
            if (players.Count > 1)
            {
                // Pick a random player to agro
                ndx = rnd.Next(0, players.Count - 1);
            }
            m.TargettingId = players[ndx].Id;
            // Nav to them - gets us close
            m.NavToX = players[ndx].X;
            m.NavToY = players[ndx].Z;
        }

        public void DoCombat()
        {
            GetMobs().Where(m => m.TargettingId != null && m.Alive).ToList().ForEach(m =>
            {
                // Check if target is still alive
                var target = _userService.ActiveUsers.Find(p => p.Id == m.TargettingId);
                if (target?.Health <= 0)
                {
                    // Target isnt alive, check for a new one
                    CheckSingleAggro(m);
                    target = _userService.ActiveUsers.Find(p => p.Id == m.TargettingId);
                }
                // If no target or too far from home, go home
                if (target == null || Math.Abs(target.X - m.MobType.Star.X) > 1000 || Math.Abs(target.Z - m.MobType.Star.Y) > 1000)
                {
                    // Wander home
                    m.NavToX = m.MobType.Star.X + rnd.Next(m.MobType.SpawnRadius * -1, m.MobType.SpawnRadius);
                    m.NavToY = m.MobType.Star.Y + rnd.Next(m.MobType.SpawnRadius * -1, m.MobType.SpawnRadius);
                    m.TargettingId = null;
                    return;
                }

                // Otherwise set wander towards mob if they're not too far away from home
                if (target.Health > 0)
                {
                    m.NavToX = target.X;
                    m.NavToY = target.Z;
                }

                // Actually attack
                var weap = new Item
                {
                    Id = new Guid().ToString(),
                    Slot = 22,
                    Damage = 5,
                    Recharge = 15,
                    Type = "launcher"
                };
                m.Weapon1CD += 25;
                if (m.Weapon1CD > 2500)
                {
                    m.Weapon1CD = 0;
                    DoAttack(target.Id, m.Id, weap);
                }
            });
        }
        public Combat DoAttack(string targetId, string attackerId, Item weapon)
        {
            var mobs = GetMobs().ToList();
            var combat = new Combat { SourceId = attackerId, TargetId = targetId, Weapon = weapon };
            var locale = new Vector2(0, 0);
            var sourcePlayer = _userService.ActiveUsers.Find(p => p?.Id == combat.SourceId);
            var sourceMob = mobs.Find(m => m.Id == combat.SourceId);
            var targetMob = mobs.Find(m => m.Id == combat.TargetId);
            combat.WeaponDamage = rnd.Next(weapon.Damage - (int)Math.Ceiling(weapon.Damage * .2), weapon.Damage + (int)Math.Ceiling(weapon.Damage * .2));
            // If target was a mob, do combat here. Otherwise do it in calling method cause players are annoying
            if (targetMob != null)
            {
                targetMob.Shield -= combat.WeaponDamage;
                if (targetMob.Shield < 0)
                {
                    targetMob.Health += targetMob.Shield;
                    targetMob.Shield = 0;
                }

                if (targetMob.Health <= 0)
                {
                    targetMob.Alive = false;
                    targetMob.KilledDate = DateTime.UtcNow;
                    combat.TargetId = "dead" + combat.TargetId;
                    _userService.AddExperience(sourcePlayer, targetMob.MobType.Level, targetMob.MobType.BonusExp, targetMob.Special);
                    var owners = new[] { sourcePlayer };
                    GenerateLoot(owners, targetMob);
                }
                combat.TargetX = targetMob.X;
                combat.TargetY = targetMob.Y;
            }

            locale = sourcePlayer != null ? new Vector2(sourcePlayer.X, sourcePlayer.Z) : locale;
            locale = sourceMob != null ? new Vector2(sourceMob.X, sourceMob.Y) : locale;
            combat.X = (int)locale.X;
            combat.Y = (int)locale.Y;
            combat.SourceX = locale.X;
            combat.SourceY = locale.Y;

            _pendingCombats.Add(combat);
            return combat;
        }

        public ICollection<Combat> GetCombats(int x, int y)
        {
            var minX = x - CombatDistance;
            var maxX = x + CombatDistance;
            var minY = y - CombatDistance;
            var maxY = y + CombatDistance;
            return _readyCombats.Where(c => c.X >= minX && c.X <= maxX && c.Y >= minY && c.Y <= maxY).ToList();
        }

        public void CycleArrays()
        {
            _readyCombats = _pendingCombats;
            _pendingCombats = new List<Combat>();
        }

        public void GenerateLoot(User[] owners, Mob Mob)
        {
            var MT = Mob.MobType;
            var minDrop = (int)Math.Floor(1 + MT.Level / 20.0f);
            var maxDrop = (int)Math.Floor(3 + MT.Level / 11.0f);
            var lootDictionary = new Dictionary<Tuple<int, int>, Item>();
            var value = 0;
            var lootValue = (MT.Level + 1) ^ 2 + 25;
            var stacks = 0;
            if (Mob.Special)
            {
                minDrop = (int)Math.Round(minDrop * 1.5);
                maxDrop += 2;
            }
            var drop = rnd.Next(minDrop, maxDrop);
            // ((i.Chance >> X) & 1) == 1) check X bit is high
            var lootItems = Globals.Items.Where(i => i.Level > MT.Level - 15 && i.Level < MT.Level + 5 && ((i.Acquisition >> 0) & 1) == 1);
            foreach (var loot in lootItems)
            {
                lootDictionary.Add(new Tuple<int, int>(value, value + loot.Cost + 1), loot);
                value += loot.Cost;
                if (loot.Stack) stacks++;
            }
            var stackValue = lootValue / stacks;
            for (var i = 0; i < drop; i++)
            {
                var rand = rnd.Next(value);
                var lootItem = lootDictionary.First(m => m.Key.Item1 < rand && m.Key.Item2 > rand).Value;
                var qty = 1;
                if (lootItem.Stack)
                {
                    var itemVal = stackValue / lootItem.Cost;
                    qty = rnd.Next(itemVal);
                }
                var newLoot = new Inventory
                {
                    Id = Guid.NewGuid().ToString(),
                    Dropped = DateTime.UtcNow,
                    ItemId = lootItem.Id,
                    Quantity = qty,
                    Slot = 100,
                    UserId = owners[0].Id,
                    X = Mob.X,
                    Y = Mob.Y
                };
                Globals.Inventory.Add(newLoot);
            }
        }

        public string GenerateName(bool special)
        {
            string[] vowels = { "a", "e", "i", "o", "u", "a", "e", "i", "o", "u", "y", "oo", "ea" };
            string[] consonants = {
                "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "n", "p", "q", "r", "s", "t", "v", "w", "x", "z",
                "ch", "nd", "qu", "rt", "ck", "st", "rr", "sl", "pl", "'", "ph"
            };
            string[] titles =
            {
                "angry","black-hearted","brooding","brute","dangerous","deadly","deathly","death-dealer","deceitful","despairing",
                "destroyer","devouring", "egregious","enraged","evil","fatal","fiery","fighter","foul","ghostly","harmful","hateful",
                "heathen","hectic","heinous","hopeless","hazardous","ignoble","ignorant","irate","jagged","jarring","jealous",
                "killer","livid","loathing","lunatic","lurker","lying","malignant","mendacious","mephitic","nag","nefarious",
                "nightmarish","objectionable","obscene","ominous","overwhelming","paradoxical","pejorative","perturbed","punisher",
                "quarrelsome","quick","resentful","sinister","sly","tank","taunting","torturous","traitorous","traumatic",
                "unholy","ungodly","unyielding","vanquisher","vengeful","violent","warrior","wicked","wretched","zealous"
            };
            string[] prefix =
            {
                "Anger",
                "Bad",
                "Black",
                "Bleak",
                "Blood",
                "Break",
                "Dare",
                "Dead",
                "Death",
                "Devil",
                "Dire",
                "Doubt",
                "Dread",
                "Empty",
                "Evil",
                "Fear",
                "Fire",
                "Flight",
                "Frost",
                "Ghost",
                "Hate",
                "Hell",
                "Hunger",
                "Ice",
                "Ire",
                "Jagged",
                "Jarring",
                "Kill",
                "Lust",
                "Metal",
                "Moon",
                "Night",
                "Null",
                "Quick",
                "Red",
                "Shadow",
                "Slander",
                "Smoke",
                "Spark",
                "Spiked",
                "Storm",
                "Strike",
                "Thorn",
                "Thunder",
                "Vile",
                "Void",
                "Wicked",
                "Zealous"
            };
            string[] suffix =
            {
                "adder",
                "blast",
                "blood",
                "breath",
                "crush",
                "death",
                "demon",
                "devil",
                "dusk",
                "ember",
                "fade",
                "fault",
                "fear",
                "fight",
                "fire",
                "flight",
                "hate",
                "jinx",
                "lightning",
                "matrix",
                "moon",
                "night",
                "nik",
                "nova",
                "null",
                "pit",
                "poison",
                "razor",
                "rex",
                "run",
                "seeker",
                "shadow",
                "smoke",
                "smolder",
                "snare",
                "soul",
                "spark",
                "spear",
                "spike",
                "star",
                "storm",
                "strike",
                "technic",
                "thunder",
                "thorn",
                "trance",
                "titan",
                "venom",
                "void",
                "wolf"
            };

            var length = rnd.Next(2, 5);

            // Given name
            var name = "";
            for (var i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0 when rnd.Next(0, 1) == 1:
                        name += vowels[rnd.Next(6)].ToUpper();
                        break;
                    case 0:
                        name += consonants[rnd.Next(19)].ToUpper();
                        break;
                    default:
                        name += vowels[rnd.Next(vowels.Length)];
                        name += consonants[rnd.Next(consonants.Length)];
                        break;
                }
            }

            // Surname
            var p = prefix[rnd.Next(prefix.Length)];
            var s = suffix[rnd.Next(suffix.Length)];

            while (string.Equals(s, p, StringComparison.CurrentCultureIgnoreCase))
            {
                s = suffix[rnd.Next(suffix.Length)];
            }

            name += " " + p + s;

            // Title
            if (special)
            {
                name += " the ";
                name += titles[rnd.Next(titles.Length)];
            }

            return name;
        }
    }
}