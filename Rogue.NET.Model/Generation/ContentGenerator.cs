using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Resources;
using System.Drawing;
using Rogue.NET.Scenario.Model;
using Rogue.NET.Scenario;
using Rogue.NET.Common;
using Microsoft.Practices.Prism.Events;
using Rogue.NET.Model.Scenario;
using Microsoft.Practices.Prism.PubSubEvents;

namespace Rogue.NET.Model.Generation
{
    public class ScenarioContentGenerator : GeneratorBase
    {
        Level[] _levels = null;
        LevelContentGenerator _generator = null;

        public ScenarioContentGenerator(IEventAggregator eventAggregator)
            : base(eventAggregator)
        {

        }

        public Level[] CreateContents(ScenarioConfiguration cfg, Level[] levels, bool survivorMode, Random rand)
        {
            _levels = levels;
            _generator = new LevelContentGenerator(cfg, survivorMode, rand);

            double progress = 0;
            for (int i = 0; i < _levels.Length; i++)
            {
                progress = (i / (double)_levels.Length) + 0.5;
                base.PublishLoadingMessage("Creating Scenario Contents", progress);

                _generator.GenerateLevelContent(_levels[i], i + 1);
            }
            return _levels;
        }
    }

    public class LevelContentGenerator
    {
        Level _level = null;
        Random _rand = null;
        ScenarioConfiguration _config = null;
        bool[,] _occupiedGrid = null;
        int _levelNumber = 0;
        bool _survivorMode = false;
        public LevelContentGenerator(ScenarioConfiguration cfg, bool survivorMode, Random rand)
        {
            _rand = rand;
            _config = cfg;
            _survivorMode = survivorMode;
        }
        public void GenerateLevelContent(Level level, int levelNum)
        {
            _level = level;
            _levelNumber = levelNum;

            //Check Constructor to see what's automatically generated
            _occupiedGrid = new bool[level.Grid.GetBounds().Right, level.Grid.GetBounds().Bottom];

            //must have for each level (Except the last one)
            if (_levelNumber != _config.DungeonTemplate.NumberOfLevels)
            {
                DoodadNormal d = new DoodadNormal(DoodadNormalType.StairsDown, "Stairs Down", "", true);
                d.Location = GetRandomCell();
                level.AddStairsDown(d);
            }

            //Stairs up - every level has one
            DoodadNormal up = new DoodadNormal(DoodadNormalType.StairsUp, "Stairs Up","", true);
            up.Location = GetRandomCell();
            level.AddStairsUp(up);

            switch (level.Type)
            {
                case LayoutType.Normal:
                    GenerateEnemies();
                    GenerateItems();
                    GenerateDoodads();
                    break;
                case LayoutType.Hall:
                    GenerateEnemies();
                    GenerateItems();
                    GenerateDoodads();
                    break;
                case LayoutType.Teleport:
                    AddTeleporterLevelContent();
                    GenerateEnemies();
                    GenerateItems();
                    GenerateDoodads();
                    break;
                case LayoutType.TeleportRandom:
                    AddTeleportRandomLevelContent();
                    GenerateEnemies();
                    GenerateItems();
                    GenerateDoodads();
                    break;
                case LayoutType.Maze:
                    GenerateEnemies();
                    GenerateItems();
                    GenerateDoodads();
                    break;
                case LayoutType.Shop:
                    level.AddDoodadNormal(new DoodadNormal(DoodadNormalType.Shop, "Mysterious Shop...", "", true));
                    break;
                case LayoutType.BigRoom:
                    GenerateEnemies();
                    GenerateItems();
                    GenerateDoodads();
                    break;
            }

            //Every level has a save point if not in survivor mode
            if (!_survivorMode)
            {
                DoodadNormal sp = new DoodadNormal(DoodadNormalType.SavePoint, "Save Point","", true);
                sp.Location = GetRandomCell();
                level.AddSavePoint(sp);
            }
            MapLevel();
        }
        private void GenerateDoodads()
        {
            foreach (DoodadTemplate t in _config.DoodadTemplates)
            {
                if (!t.Level.Contains(_levelNumber))
                    continue;

                int num = CalculateGenerationNumber(t.GenerationRate, t.IsObjectiveItem);
                for (int i = 0; i < num; i++)
                    _level.AddDoodadMagic(TemplateGenerator.GenerateDoodad(t, ref _rand));
            }
        }
        private void GenerateEnemies()
        {
            foreach (EnemyTemplate template in _config.EnemyTemplates)
            {
                EnemyTemplate et = template;
                if (!et.Level.Contains(_levelNumber))
                    continue;

                int num = CalculateGenerationNumber(et.GenerationRate, et.IsObjectiveItem);
                if (et.IsUnique)
                {
                    Enemy e = TemplateGenerator.GenerateEnemy(et, _rand);
                    if (e != null)
                        _level.AddEnemy(e);

                    continue;
                }
                for (int i = 0; i < num; i++)
                {
                    Enemy e = TemplateGenerator.GenerateEnemy(et, _rand);
                    if (e != null)
                        _level.AddEnemy(e);
                }
            }
        }
        private void GenerateItems()
        {
            foreach (EquipmentTemplate it in _config.EquipmentTemplates)
            {
                if (!it.Level.Contains(_levelNumber))
                    continue;

                int num = CalculateGenerationNumber(it.GenerationRate, it.IsObjectiveItem);
                for (int i = 0; i < num; i++)
                {
                    Equipment e = TemplateGenerator.GenerateEquipment(it, ref _rand);
                    if (e != null)
                        _level.AddEquipment(e);
                }
            }
            foreach (ConsumableTemplate t in _config.ConsumableTemplates)
            {
                if (t.Level.Contains(_levelNumber))
                {
                    int num = CalculateGenerationNumber(t.GenerationRate, t.IsObjectiveItem);
                    for (int i = 0; i < num; i++)
                    {
                        Consumable ct = TemplateGenerator.GenerateConsumable(t, ref _rand);
                        if (ct != null)
                            _level.AddConsumable(ct);
                    }
                }
            }
        }
        private int CalculateGenerationNumber(double rate, bool isobjective)
        {
            int i = (int)rate;
            double d = rate - i;
            int ct = _rand.Next(0, i + 1);
            if (d > 0 && d > _rand.NextDouble())
                ct++;

            return (isobjective && ct == 0) ? 1 : ct;
        }
        private void AddTeleporterLevelContent()
        {
            CellRectangle[] rooms = _level.Grid.GetRoomsAsArray();
            for (int i = 0; i < rooms.Length - 1; i++)
            {
                DoodadNormal d1 = new DoodadNormal(DoodadNormalType.Teleport1, "Teleport 1", "", false);
                DoodadNormal d2 = new DoodadNormal(DoodadNormalType.Teleport2, "Teleport 2", d1.Id, false);
                d1.Location = GetRandomCellInRoom(rooms[i]);
                d2.Location = GetRandomCellInRoom(rooms[i + 1]);
                d1.PairId = d2.Id;
                _level.AddDoodadNormal(d1);
                _level.AddDoodadNormal(d2);
            }
            DoodadNormal d1d = new DoodadNormal(DoodadNormalType.Teleport1, "Teleport 1", "", false);
            DoodadNormal d2d = new DoodadNormal(DoodadNormalType.Teleport2, "Teleport 2", d1d.Id, false);
            d1d.Location = GetRandomCellInRoom(rooms[rooms.Length - 1]);
            d2d.Location = GetRandomCellInRoom(rooms[0]);
            d1d.PairId = d2d.Id;
            _level.AddDoodadNormal(d1d);
            _level.AddDoodadNormal(d2d);

            //Add some extra ones
            for (int i = 0; i < 10; i++)
            {
                DoodadNormal d1 = new DoodadNormal(DoodadNormalType.Teleport1, "Teleport 1", "", false);
                DoodadNormal d2 = new DoodadNormal(DoodadNormalType.Teleport2, "Teleport 2", d1.Id, false);
                d1.Location = GetRandomCell();
                d2.Location = GetRandomCell();
                d1.PairId = d2.Id;
                _level.AddDoodadNormal(d1);
                _level.AddDoodadNormal(d2);
            }
        }
        private void AddTeleportRandomLevelContent()
        {
            CellRectangle[] rooms = _level.Grid.GetRoomsAsArray();
            for (int i = 0; i < rooms.Length; i++)
            {
                DoodadNormal d = new DoodadNormal(DoodadNormalType.TeleportRandom, "Random Teleporter", "", true);
                d.Location = GetRandomCellInRoom(rooms[i]);
                _level.AddDoodadNormal(d);
            }
        }
        private void MapLevel()
        {
            //Doodads
            for (int i = _level.GetNormalDoodads().Length - 1; i >= 0; i--)
            {
                CellPoint cp = GetRandomCell();
                if (cp == CellPoint.Empty)
                    _level.RemoveDoodadNormal(_level.GetNormalDoodads()[i]);
                else if (_level.GetNormalDoodads()[i].NormalType == DoodadNormalType.TeleportRandom)
                    continue;
                else
                    _level.GetNormalDoodads()[i].Location = cp;
            }
            for (int i = _level.GetMagicDoodads().Length - 1; i >= 0; i--)
            {
                CellPoint cp = GetRandomCell();
                if (cp == CellPoint.Empty)
                    _level.RemoveDoodadMagic(_level.GetMagicDoodads()[i]);
                else
                    _level.GetMagicDoodads()[i].Location = cp;
            }

            bool partyRoom = (_config.DungeonTemplate.PartyRoomGenerationRate > _rand.NextDouble()) && (_level.Grid.GetRoomsAsArray().Length > 0);
            CellRectangle proom = null;
            if (partyRoom)
                proom = _level.Grid.GetRandomRoom(ref _rand);

            //Items
            for (int i = _level.GetConsumables().Length - 1; i >= 0; i--)
            {
                CellPoint cp = GetRandomCell();
                if (cp == CellPoint.Empty)
                    _level.RemoveConsumable(_level.GetConsumables()[i]);
                else
                    _level.GetConsumables()[i].Location = cp;
            }
            for (int i = _level.GetEquipment().Length - 1; i >= 0; i--)
            {
                CellPoint cp = GetRandomCell();
                if (cp == CellPoint.Empty)
                    _level.RemoveEquipment(_level.GetEquipment()[i]);
                else
                    _level.GetEquipment()[i].Location = cp;
            }
            //Enemies
            for (int i = _level.GetEnemies().Length - 1; i >= 0; i--)
            {
                CellPoint cp = GetRandomCell();
                if (cp == CellPoint.Empty)
                    _level.RemoveEnemy(_level.GetEnemies()[i]);
                else
                    _level.GetEnemies()[i].Location = cp;
            }
            if (partyRoom)
            {
                foreach (ConsumableTemplate t in _config.ConsumableTemplates.Where(z => z.Level.Contains(_levelNumber)))
                {
                    int num = CalculateGenerationNumber(t.GenerationRate, t.IsObjectiveItem);
                    for (int i = 0; i < num; i++)
                    {
                        CellPoint p = GetRandomCellInRoom(proom);
                        if (p != CellPoint.Empty)
                        {
                            Consumable ic = TemplateGenerator.GenerateConsumable(t, ref _rand);
                            if (ic != null)
                            {
                                ic.Location = p;
                                _level.AddConsumable(ic);
                            }
                        }
                    }
                }
                foreach (EquipmentTemplate t in _config.EquipmentTemplates.Where(z => z.Level.Contains(_levelNumber)))
                {
                    int num = CalculateGenerationNumber(t.GenerationRate, t.IsObjectiveItem);
                    for (int i = 0; i < num; i++)
                    {
                        CellPoint p = GetRandomCellInRoom(proom);
                        if (p != CellPoint.Empty)
                        {
                            Equipment ic = TemplateGenerator.GenerateEquipment(t, ref _rand);
                            if (ic != null)
                            {
                                ic.Location = p;
                                _level.AddEquipment(ic);
                            }
                        }
                    }
                }
                List<EnemyTemplate> list = _config.EnemyTemplates.Where(z => z.Level.Contains(_levelNumber)).ToList();
                for (int i=0;i<list.Count();i++)
                {
                    int num = CalculateGenerationNumber(list[i].GenerationRate, list[i].IsObjectiveItem);
                    for (int k = 0; k < num; k++)
                    {
                        CellPoint p = GetRandomCellInRoom(proom);
                        if (p != CellPoint.Empty)
                        {
                            EnemyTemplate t = list[i];
                            Enemy ic = TemplateGenerator.GenerateEnemy(t, _rand);
                            if (ic != null)
                            {
                                ic.Location = p;
                                _level.AddEnemy(ic);
                            }
                        }
                    }
                }
            }
        }
        private CellPoint GetRandomCell()
        {
            Cell c = null;
            bool occupied = true;
            int ctr = 0;
            int ctrMax = _occupiedGrid.GetLength(0) * _occupiedGrid.GetLength(1);
            while (occupied)
            {
                c = _level.Grid.GetRandomCell(_rand);
                occupied = _occupiedGrid[c.Location.Column, c.Location.Row];
                if (++ctr >= ctrMax)
                    return CellPoint.Empty;
            }
            _occupiedGrid[c.Location.Column, c.Location.Row] = true;
            return c.Location;
        }
        private CellPoint GetRandomCellInRoom(CellRectangle room)
        {
            List<CellPoint> emptyList = new List<CellPoint>();
            for (int i = room.Left; i < room.Right; i++)
            {
                for (int j = room.Top; j < room.Bottom; j++)
                {
                    if (!_occupiedGrid[i, j] && _level.Grid.GetCell(i, j) != null)
                        emptyList.Add(_level.Grid.GetCell(i, j).Location);
                }
            }
            if (emptyList.Count == 0)
                return CellPoint.Empty;

            CellPoint pt = emptyList[_rand.Next(0, emptyList.Count)];
            _occupiedGrid[pt.Column, pt.Row] = true;
            return pt;
        }
    }
}
