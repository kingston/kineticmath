using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

using KineticMath.SubControls;

namespace KineticMath.Controllers
{
    /// <summary>
    /// Controls dynamic of the balance game
    /// </summary>
    public class BalanceGame : Game
    {
        /// <summary>
        /// Starts a new game and resets everything
        /// </summary>
        public override void StartGame()
        {
        }


        /// <summary>
        /// Called when the balance has stopped moving and is good to go
        /// </summary>
        public void VerifySolution()
        {
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

        /// <summary>
        /// Balls that are pushable
        /// </summary>
        public List<Ball> HeldBalls
        {
            get { return (List<Ball>)GetValue(HeldBallsProperty); }
            set { SetValue(HeldBallsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeldBalls.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeldBallsProperty =
            DependencyProperty.Register("HeldBalls", typeof(List<Ball>), typeof(BalanceGame), new UIPropertyMetadata(0));

        /// <summary>
        /// Balls on the left of the balance
        /// </summary>
        public List<Ball> LeftBalanceBalls
        {
            get { return (List<Ball>)GetValue(LeftBalanceBallsProperty); }
            set { SetValue(LeftBalanceBallsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LeftBalanceBalls.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LeftBalanceBallsProperty =
            DependencyProperty.Register("LeftBalanceBalls", typeof(List<Ball>), typeof(BalanceGame), new UIPropertyMetadata(0));

        /// <summary>
        /// Balls on the right of the balance
        /// </summary>
        public List<Ball> RightBalanceBalls
        {
            get { return (List<Ball>)GetValue(RightBalanceBallsProperty); }
            set { SetValue(RightBalanceBallsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RightBalanceBalls.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RightBalanceBallsProperty =
            DependencyProperty.Register("RightBalanceBalls", typeof(List<Ball>), typeof(BalanceGame), new UIPropertyMetadata(0));        
    }
}
