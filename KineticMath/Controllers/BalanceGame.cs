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

        private int currentLevel = 1;

        /// <summary>
        /// Starts a new game and resets everything
        /// </summary>
        public void NewGame()
        {
            // TODO: Load new game
            SetupLevel();
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
                LoadLevel(currentLevel);
            }
        }

        private void LoadLevel(int level)
        {
            // TODO: Load the given leve
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
        /// <param name="ball"></param>
        public void PushBall(Ball ball)
        {
            this.HeldBalls.Remove(ball);
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

        public ObservableCollection<Ball> HeldBalls { get; private set; }

        public ObservableCollection<Ball> LeftBalanceBalls { get; private set; }

        public ObservableCollection<Ball> RightBalanceBalls { get; private set; }
    }
}
