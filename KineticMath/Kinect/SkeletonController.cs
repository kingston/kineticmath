using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Kinect;
using KineticMath.Views;

namespace KineticMath.Kinect
{
    /// <summary>
    /// Controls the selection of skeletons and feeds the data to registered gesture controllers
    /// </summary>
    public class SkeletonController
    {
        private IKinectService _kinectService;
        private const int MAX_PLAYERS = 2;
        
        private int _totalPlayers;
        private GestureController[] _gestureControllers = new GestureController[MAX_PLAYERS];
        private int[] _skeletonIDs;

        public int TotalPlayers
        {
            get { return _totalPlayers; }
            set
            {
                if (value < 1 || MAX_PLAYERS < value)
                {
                    throw new ArgumentOutOfRangeException("value", value, "Total players must be between 1 and " + MAX_PLAYERS);
                }
                _totalPlayers = value;
                _skeletonIDs = new int[value];
                // Reset skeleton IDs to blank values
                ResetSkeletonIDs();
            }
        }

        public void ResetSkeletonIDs()
        {
            for (int i = 0; i < _skeletonIDs.Length; i++) _skeletonIDs[i] = -1;
        }

        public SkeletonController(IKinectService kinectService, int totalPlayers = 1)
        {
            this._kinectService = kinectService;
            this.TotalPlayers = totalPlayers;
            kinectService.SkeletonUpdated += new EventHandler<SkeletonEventArgs>(kinectService_SkeletonUpdated);
        }

        public void RegisterGestureController(int player, GestureController controller)
        {
            _gestureControllers[player - 1] = controller;
        }

        void kinectService_SkeletonUpdated(object sender, SkeletonEventArgs e)
        {
            var skels = (from s in e.Skeletons
                             where s.TrackingState == SkeletonTrackingState.Tracked
                         select s).ToList();

            // Get closest skeletons and order from left to right (facing the Kinect)
            skels = skels.OrderBy(s => s.Position.Z)
                         .Take(this.TotalPlayers)
                         .OrderBy(s => s.Position.X).ToList();
            for (int i = 0; i < _totalPlayers; i++)
            {
                // Check if we were already tracking the player
                var foundSkel = skels.FirstOrDefault(s => s.TrackingId == _skeletonIDs[i]);
                if (foundSkel == null)
                {
                    if (skels.Count == 0) break; // We ran out of skeletons

                    foundSkel = skels.First(); // Get first one off
                    skels.RemoveAt(0);
                    _skeletonIDs[i] = foundSkel.TrackingId;
                }
                else
                {
                    // We found it and can manually add it
                    skels.Remove(foundSkel);
                }
                if (_gestureControllers[i] != null)
                {
                    _gestureControllers[i].ProcessSkeleton(foundSkel);
                }
            }
        }
    }
}
