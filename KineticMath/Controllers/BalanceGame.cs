using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;

using KineticMath.SubControls;

namespace KineticMath.Controllers
{
    /// <summary>
    /// Controls dynamic of the balance game
    /// </summary>
    public class BalanceGame : DependencyObject
    {
        public BalanceGame()
        {
            HeldBalls = new ObservableCollection<Ball>();
            LeftBalanceBalls = new ObservableCollection<Ball>();
            RightBalanceBalls = new ObservableCollection<Ball>();
        }

        public event EventHandler LevelCompleted;

        private int currentLevel;

        /// <summary>
        /// Starts a new game and resets everything
        /// </summary>
        public void NewGame()
        {
            currentLevel = 1;
            LoadCurrentLevel();
        }

        /// <summary>
        /// Called when the balance has stopped moving and is good to go
        /// </summary>
        public void VerifySolution()
        {
            if (LeftBalanceBalls.Count > 0 && LeftBalanceBalls.Sum(s => s.Weight) == RightBalanceBalls.Sum(s => s.Weight))
            {
                if (LevelCompleted != null)
                {
                    LevelCompleted(this, EventArgs.Empty);
                }
                currentLevel++;
                LoadCurrentLevel();
            }
        }

        private void LoadCurrentLevel()
        {
            // TODO: Load the given level
            SetupLevel();
        }

        private void SetupLevel()
        {
            // TODO: Set up all the balls, etc. from the start parameters
            // (called during load level and reset)
        }

        /// <summary>
        /// Pushes a ball from the HeldBalls section in the assumption that it'll trigger an animation
        /// </summary>
        /// <param name="ball">The ball to remove</param>
        public bool PushBall(Ball ball)
        {
            int ballIdx = this.HeldBalls.IndexOf(ball);
            if (ballIdx == -1) return false;
            this.HeldBalls[this.HeldBalls.IndexOf(ball)] = null;
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
        public void AddBallToBalance(Ball ball, bool leftSide)
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

        public ObservableCollection<Ball> HeldBalls { get; private set; }

        public ObservableCollection<Ball> LeftBalanceBalls { get; private set; }

        public ObservableCollection<Ball> RightBalanceBalls { get; private set; }
    }
}
