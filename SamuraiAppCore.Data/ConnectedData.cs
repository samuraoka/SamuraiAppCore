﻿using Microsoft.EntityFrameworkCore;
using SamuraiAppCore.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SamuraiAppCore.Data
{
    public class ConnectedData
    {
        private SamuraiContext _context;

        public ConnectedData()
        {
            _context = new SamuraiContext();
            _context.Database.EnsureCreated();
        }

        public Samurai CreateNewSamurai()
        {
            // battles (many to many) will not be involved
            var samurai = new Samurai { Name = "New Samurai" };
            _context.Samurais.Add(samurai);
            return samurai;
        }

        public ObservableCollection<Samurai> SamuraisListInMemory()
        {
            if (_context.Samurais.Local.Count == 0)
            {
                _context.Samurais.ToList();
            }
            return _context.Samurais.Local.ToObservableCollection();
        }

        public Samurai LoadSamuraiGraph(int samuraiId)
        {
            var samurai = _context.Samurais.Find(samuraiId);
            _context.Entry(samurai).Reference(s => s.SecretIdentity).Load();
            _context.Entry(samurai).Collection(s => s.Quotes).Load();
            return samurai;
        }

        public void SaveChanges(Type typeBeingEdited)
        {
            _context.SaveChanges();
            if (typeBeingEdited == typeof(Samurai))
            {
                if (_context.Samurais.Local.Any())
                {
                    SamuraisListInMemory().ToList().ForEach(s => s.IsDirty = false);
                }
            }
            if (typeBeingEdited == typeof(Battle))
            {
                if (_context.Battles.Local.Any())
                {
                    BattlesListInMemory().ToList().ForEach(b => b.IsDirty = false);
                }
            }
        }

        public ObservableCollection<Battle> BattlesListInMemory()
        {
            if (_context.Battles.Local.Count == 0)
            {
                _context.Battles.ToList();
            }
            return _context.Battles.Local.ToObservableCollection();
        }

        public List<Samurai> SamuraisNotInBattle(int battleId)
        {
            var existingSamurais = _context.SamuraiBattles
                .Where(sb => sb.BattleId == battleId)
                .Select(sb => sb.SamuraiId).ToList();
            var samurais = _context.Samurais.AsNoTracking()
                .Where(s => existingSamurais.Contains(s.Id) == false)
                .ToList();
            return samurais;
        }

        public Battle LoadBattleGraph(int battleId)
        {
            var battle = _context.Battles.Find(battleId);
            _context.Entry(battle).Collection(b => b.SamuraiBattles).Query().Include(sb => sb.Samurai).Load();
            return battle;
        }

        public SamuraiBattle AddSamuraiBattle(SamuraiBattle samuraiBattle)
        {
            // presumes samurai and battle always already exist
            var sb = _context.SamuraiBattles.Find(samuraiBattle.Samurai.Id, samuraiBattle.Battle.Id) ?? samuraiBattle;
            var entityentry = _context.Entry(sb);
            if (entityentry.State == EntityState.Detached)
            {
                entityentry.State = EntityState.Added;
            }
            return sb;
        }

        public bool IsCommittedSamuraiBattle(SamuraiBattle samuraiBattle)
        {
            bool ret = false;
            var sb = _context.SamuraiBattles.Find(samuraiBattle.SamuraiId, samuraiBattle.BattleId);
            ret = sb != null;
            if (sb != null)
            {
                var entityEntry = _context.Entry(sb);
                if (entityEntry.State == EntityState.Added)
                {
                    ret = false;
                }
            }
            return ret;
        }

        public void DetachUnCommittedSamuraiBattle(SamuraiBattle samuraiBattle)
        {
            var entityEntry = _context.Entry(samuraiBattle);
            entityEntry.State = EntityState.Detached;
        }

        public void RevertBattleChanges(int id)
        {
            // this is the simplest way.
            // Maybe later version of EF will make it easier
            _context = new SamuraiContext();
        }

        public Battle CreateNewBattle()
        {
            // samurais (many to many) will not be involved
            var battle = new Battle { Name = "New Battle" };
            _context.Battles.Add(battle);
            return battle;
        }
    }
}
