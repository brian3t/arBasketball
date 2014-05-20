using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Animation))]
public class Player : MonoBehaviour
{

		public Ball basketBall;
		public float bounceForce = 1000f;
		private Transform _transform;
		private AnimationClip _currentAnimation = null;
		public AnimationClip animIdle;
		public AnimationClip animBounceDown;
		public AnimationClip animBounceUp;
		public AnimationClip animWalkForward;
		public AnimationClip animWalkBackward;
		public AnimationClip animPrepareThrow;
		public AnimationClip animThrow;
		public AnimationClip animScore;
		public AnimationClip animMiss;
		public float maxThrowForce = 5000f;
		public Vector3 throwDirection = new Vector3 (-1.0f, 0.5f, 0.0f);
		public float walkSpeed = 5.0f;


		public delegate void PlayerAnimationFinished (string animation);

		public PlayerAnimationFinished OnPlayerAnimationFinished = null;
		private Vector3 _shotPosition = Vector3.zero;


}
