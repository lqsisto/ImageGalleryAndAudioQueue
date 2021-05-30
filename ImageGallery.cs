using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace ImageGallery
{
	/// <summary>
	/// Show queue of images and automatically move to next one
	/// </summary>
	public class ImageGallery : MonoBehaviour
	{
		
		[Space, SerializeField, Tooltip ("If true will start image gallery when enabled")]
		private bool startImageGalleryWhenEnabled;
		
		[Space, Tooltip ("How long should fade between images last"), SerializeField]
		private float transitionTime = 0.5f;

		[Tooltip ("If true, will loop images indefinitely"), SerializeField]
		private bool loop;

		[Tooltip ("The color to which image is faded"), SerializeField]
		private Color fadeColor;

		[Space, SerializeField] private Image galleryImage;

		[Space, Tooltip ("Create image gallery objects here"), SerializeField]
		private List <GalleryImage> galleryImages = new List <GalleryImage> ();

		[Space,
		 Tooltip (
			 "If image gallery contains animations this is needed.\nIf gallery contains no animations this can be null"),
		 SerializeField]
		private Animator animator;

		//Holder for current image index
		private int currentImageIndex;

		//By changing this to true image gallery is started
		private bool startImageGallery;

		//Helper variable to return early in Update() if tween is active 
		private bool doingTween;

		//Helper variable to fade between images in Update()
		private float timeStarted;


        //Gallery image dataobject
		[System.Serializable]
		private struct GalleryImage
		{
			public string animatorTrigger;
			public Sprite sprite;
			public float timeToShow;
		}


		//Set default values
		private void Reset ()
		{
			transitionTime = 0.5f;
			loop = false;
			fadeColor = Color.black;
		}

        public void StartImageGallery ()
		{
			Internal_StartImageGallery ();
		}

		private void Internal_StartImageGallery ()
		{
			startImageGallery = true;
			MoveNext (0, true);
		}

		private void OnEnable()
		{
			galleryImage.preserveAspect = true;

			if(startImageGalleryWhenEnabled)
				StartImageGallery();
		}

		private void Update ()
		{
			//Check if it's okay to switch image
			if (!startImageGallery || doingTween || timeStarted + galleryImages [currentImageIndex].timeToShow > Time.time
			)
			{
				return;
			}


			if (currentImageIndex == galleryImages.Count - 1)
			{
				//if image gallery will loop images set index back to 0 when all images are looped though
				if (loop)
				{
					currentImageIndex = 0;
					MoveNext (currentImageIndex++, false);
					return;
				}

				EndActivity ();
				return;
			}

			MoveNext (++currentImageIndex, false);
		}


		/// <summary>
		/// Move to next image in image gallery.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="firstTime"></param>
		private void MoveNext (int index, bool firstTime)
		{
			//if entering first time set image to transparent and fade from there
			if (firstTime)
			{
				galleryImage.color = new Color (0, 0, 0, 0);
				galleryImage.enabled = true;

				SetImage (galleryImages [index]);

				var startTween = galleryImage.DOColor (Color.white, transitionTime);

				startTween.OnStart (() => doingTween = true);
				startTween.onComplete += delegate
				{
					doingTween = false;

					currentImageIndex = index;
					timeStarted = Time.time;
				};
			}
			else
			{
				//tween image to fadecolor and back and switch image between. This creates nice transition effect :)
				var tween = galleryImage.DOColor (fadeColor, transitionTime).From (Color.white);
				tween.Play ();
				tween.OnStart (() => doingTween = true);

				tween.onComplete += delegate
				{
					SetImage (galleryImages [index]);
					tween = galleryImage.DOColor (Color.white, transitionTime).From (fadeColor);

					doingTween = false;

					currentImageIndex = index;
					timeStarted = Time.time;
				};
			}
		}


		/// <summary>
		/// Switch sprite on gallery image
		/// </summary>
		/// <param name="sprite"></param>
		private void SetImage (GalleryImage g)
		{
			galleryImage.sprite = g.sprite;


			if (!animator)
				return;

			if (g.animatorTrigger.Equals (""))
			{
				animator.enabled = false;
				return;
			}

			animator.enabled = true;
			animator.SetTrigger (g.animatorTrigger);
		}

		/// <summary>
		/// Disable the component
		/// </summary>
		private void EndActivity ()
		{
			enabled = false;
		}
	}
}