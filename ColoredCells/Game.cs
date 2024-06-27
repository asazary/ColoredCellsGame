namespace ColoredCells
{
    internal enum GameState
    {
        Continues = 0,
        Paused,
        Finished
    }

    internal class Game
    {
        public int[,] Field { get; private set; } = new int[1, 1] { { 0 } };
        public int Size { get; private set; } = 3;
        public int Level { get; private set; } = 1;
        public int ColorsCount { get; private set; } = 2;
        
        public GameState GameStatus { get; private set; } = GameState.Finished;
        private readonly int _randomMovesCount = 1;

        private static readonly Func<int, int, int>[] AllCellFuncs =
        [
            (x, clrCnt) => x + 1 <= clrCnt ? x + 1 : 1,             // level 0
            (x, clrCnt) => x - 1 >= 1 ? x - 1 : clrCnt,             // level 1
            (x, clrCnt) => x + 2 <= clrCnt ? x + 2 : x + 2 - clrCnt,// level 2            
            (x, clrCnt) => x + 1 <= clrCnt ? x + 1 : 1,             // level 3
            (x, clrCnt) => x - 2 >= 1 ? x - 2 : clrCnt + x - 2      // level 4            
        ];

        private readonly List<Action<int, int, List<CellCoords>>> AllLevelFuncs = [];
        private readonly Action<int, int, List<CellCoords>> GameLevelFuncs = delegate { };


        public Game(int size, int colorsCount, int level, int randomMovesCount)
        {
            Size = size;
            Field = new int[size, size];
            Level = level;
            ColorsCount = colorsCount;
            _randomMovesCount = randomMovesCount;

            AllLevelFuncs.Add(ApplyLevel0);
            AllLevelFuncs.Add(ApplyLevel1);
            AllLevelFuncs.Add(ApplyLevel2);
            AllLevelFuncs.Add(ApplyLevel3);
            AllLevelFuncs.Add(ApplyLevel4);

            for (var i = 0; i <= level; i++)
                GameLevelFuncs += AllLevelFuncs[i];            
        }

        public void StartGame()
        {
            GameStatus = GameState.Paused;
            InitCellsWithRandomValue();
            DoRandomMoves();
            GameStatus = GameState.Continues;
        }
        public void EndGame() => GameStatus = GameState.Finished;
        public void PauseGame() => GameStatus = GameState.Paused;
        public void ContinueGame() => GameStatus = GameState.Continues;

        public bool CheckWin()
        {
            var val = Field[0, 0];
            for (var i = 0; i < Size; i++)
                for (var j = 0; j < Size; j++)
                    if (Field[i, j] != val) return false;
            return true;
        }

        public List<CellCoords> ChangeCell(CellCoords coords)
        {
            if (GameStatus != GameState.Continues && 
                  GameStatus != GameState.Paused) return [];

            var (row, col) = (coords.Row, coords.Col);
            var changedCells = new List<CellCoords>();
            GameLevelFuncs(row, col, changedCells);
            return changedCells;
        }

        public void InitCellsWithRandomValue() 
        {
            var initValue = new Random().Next(1, ColorsCount + 1);
            for (var i = 0; i < Size; i++)
                for (var j = 0; j < Size; j++)
                    Field[i, j] = initValue;
        }


        public void DoRandomMoves()
        {
            var rand = new Random();
            for (var i = 0; i < _randomMovesCount; i++)
            {
                var row = rand.Next(Size);
                var col = rand.Next(Size);
                ChangeCell(new CellCoords(row, col));
            }
        }

        private void ApplyLevel0(int row, int col, List<CellCoords> changedCells)
        {
            Field[row, col] = AllCellFuncs[0](Field[row, col], ColorsCount);
            changedCells.Add(new CellCoords(row, col));
        }            

        private void ApplyLevel1(int row, int col, List<CellCoords> changedCells)
        {
            var toChange = new (int r, int c)[] { (row, col - 1), (row, col + 1) };
            foreach (var (r, c) in toChange)
            {
                if (!IsCoordCorrect(r, c)) continue;
                Field[r, c] = AllCellFuncs[1](Field[r, c], ColorsCount);
                changedCells.Add(new CellCoords(r, c));
            }
        }

        private void ApplyLevel2(int row, int col, List<CellCoords> changedCells)
        {
            var toChange = new (int r, int c)[] { (row - 1, col), (row + 1, col) };
            foreach (var (r, c) in toChange)
            {
                if (!IsCoordCorrect(r, c)) continue;
                Field[r, c] = AllCellFuncs[2](Field[r, c], ColorsCount);
                changedCells.Add(new CellCoords(r, c));
            }
        }

        private void ApplyLevel3(int row, int col, List<CellCoords> changedCells)
        {
            var toChange = new (int r, int c)[] { (row - 1, col + 1), (row + 1, col - 1) };
            foreach (var (r, c) in toChange)
            {
                if (!IsCoordCorrect(r, c)) continue;
                Field[r, c] = AllCellFuncs[3](Field[r, c], ColorsCount);
                changedCells.Add(new CellCoords(r, c));
            }

        }

        private void ApplyLevel4(int row, int col, List<CellCoords> changedCells)
        {
            var toChange = new (int r, int c)[] { (row + 1, col + 1), (row - 1, col - 1) };
            foreach (var (r, c) in toChange)
            {
                if (!IsCoordCorrect(r, c)) continue;
                Field[r, c] = AllCellFuncs[4](Field[r, c], ColorsCount);
                changedCells.Add(new CellCoords(r, c));
            }
        }

        private bool IsCoordCorrect(int row, int col) =>
            row >= 0 && row < Size && col >= 0 && col < Size;
        
    }
}
