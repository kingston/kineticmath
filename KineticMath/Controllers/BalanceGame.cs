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
        public static int MINUTE = 60;
        public static int TEN_SECOND = 10;

        DispatcherTimer timer;
        public Mode mode;

        public enum Mode
        {
            Challenge,
            Practice,
            Classic
        }

        public int Score { get; set; }

        /// <summary>
        /// The time left in the challenge mode in seconds
        /// </summary>
        public int TimeLeft { get; set; }

        public int LivesLeft { get; protected set; }

        public BalanceGame()
        {
            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(timerHandler);
            mode = Mode.Classic;
            HeldBalls = new ObservableCollection<SeesawObject>();
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

        /// <summary>
        /// Starts a new game and resets everything
        /// </summary>
        private void timerHandler(Object sender, EventArgs args)
        {
            if (this.TimeLeft >= 0)
            {
                TimerTicked(this, EventArgs.Empty);
                timer.Interval = TimeSpan.FromSeconds(1);
                timer.Start();
                this.TimeLeft--;
            }
            else
            {
                if (mode == Mode.Classic)
                {
                    LivesLeft--;
                    LevelLost(this, new LevelLostEventArgs(LevelLostEventArgs.Reason.TimeUp));
                    if (LivesLeft > 0)
                    {
                        TimeLeft = TEN_SECOND;
                    }
                    else
                    {
                        timer.Stop();
                        GameOver(this, EventArgs.Empty);
                    }
                }
                else if (mode == Mode.Challenge)
                {
                    LivesLeft--;
                    timer.Stop();
                    GameOver(this, EventArgs.Empty);
                }
            }
        }

        public void setMode(Mode mode)
        {
            this.mode = mode;
        }

        /// <summary>
        /// Starts a new game and resets everything
        /// </summary>
        public void NewGame()
        {
            if (mode == Mode.Challenge)
            {
                if (timer.IsEnabled)
                {
                    timer.Stop(); // Reset timer
                }
                this.LivesLeft = 1;
                this.TimeLeft = MINUTE;
                Score = 0;
                currentLevel = 1;
                LoadCurrentLevel();
                timer.Interval = TimeSpan.FromSeconds(1);
                timer.Start();
            }
            else if (mode == Mode.Classic)
            {
                if (timer.IsEnabled)
                {
                    timer.Stop(); // Reset timer
                }
                this.LivesLeft = 3;
                this.TimeLeft = TEN_SECOND;
                Score = 0;
                currentLevel = 1;
                LoadCurrentLevel();
                timer.Interval = TimeSpan.FromSeconds(1);
                timer.Start();
            }
            else
            {
                if (timer.IsEnabled)
                {
                    timer.Stop();
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
                        if (mode == Mode.Challenge)
                        {
                            TimeLeft += Convert.ToInt32(Math.Log(currentLevel) * 5);
                        }
                        currentLevel++;
                    }
                }
                else
                {
                    if (mode != Mode.Practice)
                    {
                        this.LivesLeft--;
                    }
                    if (LivesLeft <= 0)
                    {
                        timer.Stop();
                        if (GameOver != null)
                        {
                            GameOver(this, EventArgs.Empty);
                        }
                    }
                    else if (LevelLost != null)
                    {
                        LevelLost(this, new LevelLostEventArgs(LevelLostEventArgs.Reason.WrongAnswer));
                    }
                }
            }
            if (mode == Mode.Classic)
                TimeLeft = TEN_SECOND;
        }

        private int[] curBalls;
        private int[] targetRightSide;

        public void LoadCurrentLevel()
        {
            switch (currentLevel)
            {
                case 1:
                    curBalls = new int[] { 2, 4, 5, 6 };
                    targetRightSide = new int[] { 2, 3 };
                    break;
                case 2:
                    curBalls = new int[] { 5, 7, 3, 6 };
                    targetRightSide = new int[] { 5, 1 };
                    break;
                case 3:
                    curBalls = new int[] { 2, 4, 6, 3 };
                    targetRightSide = new int[] { 4, 2 };
                    break;
                default:
                    Random rand = new Random();
                    // Generate the answer options
                    int rightSideNum = 2;
                    if (currentLevel < 10)
                        rightSideNum = 2;
                    else if (currentLevel < 20)
                        rightSideNum = 3;
                    else
                        rightSideNum = 4;
                    targetRightSide = new int[rightSideNum];
                    int i = 0;
                    int max = 0;
                    if (currentLevel < 10)
                        max = 10;
                    else
                        max = currentLevel / 10 * 10;

                    while (i < targetRightSide.Length)
                    {
                        int candidate = rand.Next(1, currentLevel * 3 - 3);
                        if (!targetRightSide.Contains(candidate))
                        {
                            targetRightSide[i] = candidate;
                            i++;
                        }
                    }
                    i = 0;
                    curBalls = new int[4];
                    int answer = targetRightSide.Sum();
                    while (i < curBalls.Length)
                    {
                        int candidate = rand.Next(Convert.ToInt32(Math.Min(answer * 0.5, 1)), answer * 2);
                        if (!curBalls.Contains(candidate) && candidate != answer)
                        {
                            curBalls[i] = candidate;
                            i++;
                        }
                    }
                    curBalls[rand.Next(0, curBalls.Length)] = answer;
                    break;
            }
            SetupLevel();
        }

        private void SetupLevel()
        {
            if (LevelReset != null) LevelReset(null, EventArgs.Empty);
            HeldBalls.Clear();
            LeftBalanceBalls.Clear();
            RightBalanceBalls.Clear();
            foreach (var weight in curBalls)
            {
                HeldBalls.Add(new Bird(weight.ToString(), weight));
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
            return Math.Max(curBalls.Sum(), targetRightSide.Sum());
        }

        /// <summary>
        /// Pushes a ball from the HeldBalls section in the assumption that it'll trigger an animation
        /// </summary>
        /// <param name="ball">The ball to remove</param>
        public bool PushBall(SeesawObject ball)
        {
            if (ball == null) return false;
            int ballIdx = this.HeldBalls.IndexOf(ball);
            if (ballIdx == -1) return false;
            this.HeldBalls[ballIdx] = null;
            return true;
        }

        /// <summary>
        /// Resets the board to the current level
        /// </summary>
        public void Reset()
        {
            SetupLevel();
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

        public ObservableCollection<SeesawObject> HeldBalls { get; private set; }

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
