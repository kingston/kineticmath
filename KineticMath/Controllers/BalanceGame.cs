using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;

using KineticMath.SubControls;
using System.Windows.Threading;
using KineticMath.Views;

namespace KineticMath.Controllers
{
    /// <summary>
    /// Controls dynamic of the balance game
    /// </summary>
    public class BalanceGame : Game
    {
        /// <summary>
        /// Total time for a challenge game
        /// </summary>
        private const int CHALLENGE_TIME = 60;

        /// <summary>
        /// Time for a classic level
        /// </summary>
        private int _classicLevelTime;

        /// <summary>
        /// The timer that keeps track of time left for each level
        /// </summary>
        private DispatcherTimer gameTimer;
        /// <summary>
        /// The current mode of the game
        /// </summary>
        private Mode currentMode;

        /// <summary>
        /// The current mode of the game
        /// </summary>
        public Mode CurrentMode
        {
            get
            {
                return this.currentMode;
            }
            set
            {
                this.currentMode = value;
                this.NewGame();
            }
        }

        public enum Mode
        {
            Challenge,
            Practice,
            Classic
        }

        /// <summary>
        /// The score of the current player
        /// </summary>
        public int Score { get; protected set; }

        /// <summary>
        /// The time left in the challenge mode in seconds
        /// </summary>
        public int TimeLeft { get; protected set; }

        /// <summary>
        /// The number of lives left of the player
        /// </summary>
        public int LivesLeft { get; protected set; }

        public BalanceGame()
        {
            gameTimer = new DispatcherTimer();
            gameTimer.Tick += new EventHandler(timerHandler);
            currentMode = Mode.Classic;
            PlayerOneHeldBalls = new ObservableCollection<SeesawObject>();
            PlayerTwoHeldBalls = new ObservableCollection<SeesawObject>();
            LeftBalanceBalls = new ObservableCollection<SeesawObject>();
            RightBalanceBalls = new ObservableCollection<SeesawObject>();
        }

        public event EventHandler TimerTicked;

        public event EventHandler LevelCompleted;

        /// <summary>
        /// Called when they got the wrong answer
        /// </summary>
        public event EventHandler LevelLost;

        /// <summary>
        /// Called when the level is reset and should end all animations that would add balls
        /// </summary>
        public event EventHandler LevelReset;

        public event EventHandler GameOver;

        private int currentLevel;

        private bool _twoPlayerMode = false;

        public bool TwoPlayerMode
        {
            get
            {
                return _twoPlayerMode;
            }
            set
            {
                _twoPlayerMode = value;
            }
        }

        /// <summary>
        /// Starts a new game and resets everything
        /// </summary>
        private void timerHandler(Object sender, EventArgs args)
        {
            if (this.TimeLeft >= 0)
            {
                TimerTicked(this, EventArgs.Empty);
                gameTimer.Interval = TimeSpan.FromSeconds(1);
                gameTimer.Start();
                this.TimeLeft--;
            }
            else
            {
                if (currentMode == Mode.Classic)
                {
                    LivesLeft--;
                    if (LivesLeft > 0)
                    {
                        LevelLost(this, new LevelLostEventArgs(LevelLostEventArgs.Reason.TimeUp));
                        this.TimeLeft = _classicLevelTime;
                    }
                    else
                    {
                        gameTimer.Stop();
                        GameOver(this, EventArgs.Empty);
                    }
                }
                else if (currentMode == Mode.Challenge)
                {
                    LivesLeft--;
                    gameTimer.Stop();
                    GameOver(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Starts a new game and resets everything
        /// </summary>
        public void NewGame()
        {
            if (_twoPlayerMode) _classicLevelTime = 20;
            else _classicLevelTime = 10;
            if (currentMode == Mode.Challenge)
            {
                if (gameTimer.IsEnabled)
                {
                    gameTimer.Stop(); // Reset timer
                }
                this.LivesLeft = 1;
                this.TimeLeft = CHALLENGE_TIME;
                Score = 0;
                currentLevel = 1;
                LoadCurrentLevel();
                gameTimer.Interval = TimeSpan.FromSeconds(1);
                gameTimer.Start();
            }
            else if (currentMode == Mode.Classic)
            {
                if (gameTimer.IsEnabled)
                {
                    gameTimer.Stop(); // Reset timer
                }
                this.LivesLeft = 3;
                this.TimeLeft = _classicLevelTime;
                Score = 0;
                currentLevel = 1;
                LoadCurrentLevel();
                gameTimer.Interval = TimeSpan.FromSeconds(1);
                gameTimer.Start();
            }
            else
            {
                if (gameTimer.IsEnabled)
                {
                    gameTimer.Stop();
                }
                this.LivesLeft = 1;
                currentLevel = 1;
                LoadCurrentLevel();
            };
        }

        /// <summary>
        /// Called when the balance has stopped moving and is good to go
        /// </summary>
        public void VerifySolution()
        {
            if (LeftBalanceBalls.Count > 0)
            {
                if (LeftBalanceBalls.Sum(s => s.Weight) == RightBalanceBalls.Sum(s => s.Weight))
                {
                    if (LevelCompleted != null)
                    {
                        Score++;
                        LevelCompleted(this, EventArgs.Empty);
                        // Let's make life interesting ;)
                        if (currentMode == Mode.Challenge)
                        {
                            TimeLeft += Convert.ToInt32(Math.Log(currentLevel) * 2);
                        }
                        currentLevel++;
                    }
                    if (currentMode == Mode.Classic)
                        TimeLeft = _classicLevelTime;
                }
                else if (!_twoPlayerMode || RightBalanceBalls.Any(s => s is Bird))
                {
                    if (currentMode == Mode.Classic)
                    {
                        this.LivesLeft--;
                    }
                    if (LivesLeft > 0)
                    {
                        if (LevelLost != null)
                        {
                            LevelLost(this, new LevelLostEventArgs(LevelLostEventArgs.Reason.WrongAnswer));
                        }
                    }
                    else
                    {
                        gameTimer.Stop();
                        if (GameOver != null)
                        {
                            GameOver(this, EventArgs.Empty);
                        }
                    }
                    if (currentMode == Mode.Classic)
                        TimeLeft = _classicLevelTime;

                }
            }
        }

        private int[] curOneBalls;
        private int[] curTwoBalls;
        private int[] targetRightSide;

        public void LoadCurrentLevel()
        {
            switch (currentLevel)
            {
                case 1:
                    if (_twoPlayerMode)
                    {
                        curOneBalls = new int[] { 2, 4, 5, 7 };
                        curTwoBalls = new int[] { 1, 2, 3, 4 };
                        targetRightSide = new int[] { 2, 4 };
                    }
                    else
                    {
                        curOneBalls = new int[] { 2, 4, 5, 6 };
                        curTwoBalls = new int[] { };
                        targetRightSide = new int[] { 2, 3 };
                    }
                    break;
                case 2:
                    if (_twoPlayerMode)
                    {
                        curOneBalls = new int[] { 8, 10, 6, 9 };
                        curTwoBalls = new int[] { 1, 2, 4, 3 };
                        targetRightSide = new int[] { 5, 2 };
                    }
                    else
                    {
                        curOneBalls = new int[] { 5, 7, 3, 6 };
                        curTwoBalls = new int[] { };
                        targetRightSide = new int[] { 5, 1 };
                    }
                    break;
                case 3:
                    if (_twoPlayerMode)
                    {
                        curOneBalls = new int[] { 2, 4, 6, 5 };
                        curTwoBalls = new int[] { 2, 3, 5, 6 };
                        targetRightSide = new int[] { 1, 2 };
                    }
                    else
                    {
                        curOneBalls = new int[] { 2, 4, 6, 3 };
                        curTwoBalls = new int[] { };
                        targetRightSide = new int[] { 4, 2 };
                    }
                    break;
                default:
                    Random rand = new Random();
                    // If we're starting with no target right side
                    if (targetRightSide == null) targetRightSide = new int[] { currentLevel * 2 / 3 };
                    int prevAnswer = targetRightSide.Sum();
                    // Generate the answer options
                    int rightSideNum = 2;
                    if (currentLevel < 10)
                        rightSideNum = 2;
                    else if (currentLevel < 20)
                        rightSideNum = 3;
                    else
                        rightSideNum = 4;
                    int targetAnswer = prevAnswer + rand.Next(2, Math.Max(currentLevel * 2 / 3, 4));
                    targetRightSide = GetRandomPartsToSum(rightSideNum, targetAnswer);

                    int secondPlayerPart = 0;
                    if (_twoPlayerMode)
                    {
                        // Add second player's parts as well
                        secondPlayerPart = rand.Next(2, Math.Min(currentLevel - 3, 2) * 2);

                        // Second player's answer set
                        List<int> playerTwoAnswers = GetAnswerSet(secondPlayerPart);

                        curTwoBalls = new int[4];
                        for (int i = 0; i < 4; i++)
                        {
                            int index = rand.Next(0, playerTwoAnswers.Count - 1);
                            curTwoBalls[i] = playerTwoAnswers[index];
                            playerTwoAnswers.RemoveAt(index);
                        }
                    }
                    else curTwoBalls = new int[0];

                    int answer = targetRightSide.Sum() + secondPlayerPart;

                    // Build answer set
                    List<int> answerSet = GetAnswerSet(answer);

                    curOneBalls = new int[4];
                    for (int i = 0; i < 4; i++)
                    {
                        int index = rand.Next(0, answerSet.Count - 1);
                        curOneBalls[i] = answerSet[index];
                        answerSet.RemoveAt(index);
                    }
                    break;
            }
            SetupLevel();
        }

        private List<int> GetAnswerSet(int answer)
        {
            Random rand = new Random();
            List<int> answerSet = new List<int>();
            int randOffset, sign;
            // 0: the real answer
            answerSet.Add(answer);
            // 1: answer +- 1~3
            randOffset = rand.Next(1, 3);
            sign = answer - randOffset > 1 ? (rand.Next(1, 100) < 50 ? -1 : 1) : 1;
            answerSet.Add(answer + sign * randOffset);
            // 2: answer +- 10
            randOffset = 10;
            sign = answer - randOffset > 1 ? (rand.Next(1, 100) < 50 ? -1 : 1) : 1;
            answerSet.Add(answer + sign * randOffset);
            // 3: answer +- 10 +- 1~3
            randOffset += rand.Next(1, 3);
            sign = answer - randOffset > 1 ? (rand.Next(1, 100) < 50 ? -1 : 1) : 1;
            answerSet.Add(answer + sign * randOffset);
            return answerSet;
        }

        /// <summary>
        /// Produces a random array of integers that sum to a target sum
        /// </summary>
        /// <param name="targetSum"></param>
        /// <returns></returns>
        private int[] GetRandomPartsToSum(int numParts, int targetSum)
        {
            int[] parts = new int[numParts];
            Random rand = new Random();
            for (int i = 0; i < parts.Length - 1; i++)
            {
                parts[i] = targetSum / numParts;
            }
            parts[parts.Length - 1] = targetSum - parts.Sum();

            for (int i = 0; i < parts.Length - 1; i++)
            {
                int randMove = rand.Next(0, Math.Max(parts[i] / 2, 1));
                int side = rand.NextDouble() < 0.5 ? -1 : 1;
                parts[i] += side * randMove;
                parts[i + 1] += -side * randMove;
            }
            return parts;
        }

        private void SetupLevel()
        {
            if (LevelReset != null) LevelReset(null, EventArgs.Empty);
            PlayerOneHeldBalls.Clear();
            PlayerTwoHeldBalls.Clear();
            LeftBalanceBalls.Clear();
            RightBalanceBalls.Clear();
            foreach (var weight in curOneBalls)
            {
                PlayerOneHeldBalls.Add(new Bird(weight.ToString(), weight));
            }
            if (curTwoBalls != null)
            {
                foreach (var weight in curTwoBalls)
                {
                    PlayerTwoHeldBalls.Add(new Bird(weight.ToString(), weight));
                }
            }
            foreach (var weight in targetRightSide)
            {
                RightBalanceBalls.Add(new Pig(weight.ToString(), weight));
            }
        }

        /// <summary>
        /// Gets the maximum value one side can have (used by Seesaw for angle adjusting)
        /// </summary>
        /// <returns></returns>
        public int GetMaximumValue()
        {
            return Math.Max(curOneBalls.Sum(), targetRightSide.Sum() + (curTwoBalls ?? new int[] {}).Sum());
        }

        /// <summary>
        /// Pushes a ball from the HeldBalls section in the assumption that it'll trigger an animation
        /// </summary>
        /// <param name="ball">The ball to remove</param>
        public bool PushBall(SeesawObject ball)
        {
            if (ball == null) return false;
            int ballOneIdx = this.PlayerOneHeldBalls.IndexOf(ball);
            int ballTwoIdx = this.PlayerTwoHeldBalls.IndexOf(ball);

            if (ballOneIdx > -1) this.PlayerOneHeldBalls[ballOneIdx] = null;
            else if (ballTwoIdx > -1) this.PlayerTwoHeldBalls[ballTwoIdx] = null;
            else return false;

            return true;
        }

        /// <summary>
        /// Resets the board to the current level
        /// </summary>
        public void Reset()
        {
            SetupLevel();
            if (currentMode == Mode.Classic)
            {
                if (LivesLeft > 0)
                {
                    TimeLeft = _classicLevelTime;
                }
            }
        }

        /// <summary>
        /// Adds a ball to the balance
        /// </summary>
        /// <param name="ball"></param>
        /// <param name="leftSide"></param>
        public void AddBallToBalance(SeesawObject ball, bool leftSide)
        {
            if (leftSide)
            {
                LeftBalanceBalls.Add(ball);
            }
            else
            {
                RightBalanceBalls.Add(ball);
            }
        }

        // Gets the amount of weight the right side is heavier than the left
        public double GetBalanceOffset()
        {
            return RightBalanceBalls.Sum(s => s.Weight) - LeftBalanceBalls.Sum(s => s.Weight);
        }

        public ObservableCollection<SeesawObject> PlayerOneHeldBalls { get; private set; }

        public ObservableCollection<SeesawObject> PlayerTwoHeldBalls { get; private set; }

        public ObservableCollection<SeesawObject> LeftBalanceBalls { get; private set; }

        public ObservableCollection<SeesawObject> RightBalanceBalls { get; private set; }

    }

    public class LevelLostEventArgs : EventArgs
    {
        public enum Reason
        {
            TimeUp, WrongAnswer
        }
        public Reason reason { get; set; }
        public LevelLostEventArgs(Reason r)
        {
            reason = r;
        }
    }
}
