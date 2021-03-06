﻿using UnityEngine;
 using System.Collections;
 
 // @NOTE the attached sprite's position should be "top left" or the children will not align properly
 // Strech out the image as you need in the sprite render, the following script will auto-correct it when rendered in the game
 [RequireComponent (typeof (SpriteRenderer))]
 
 // Generates a nice set of repeated sprites inside a streched sprite renderer
 // @NOTE Vertical only, you can easily expand this to horizontal with a little tweaking
 public class RepeatSpriteBoundary : MonoBehaviour {
	 public Sprite spriteImg;
	 public CameraController mainCamera;
	 public float moveIntervalFactor = 1;
	 public float tileScalex = 1;

	 private int lastChildIndex;
	 public int ntiles;
     private SpriteRenderer sprite;
	 private float moveInterval;
	 private SpriteRenderer childSprite;
	 private float childSpriteSizeX;
	//  private float childSpriteSizeY;
	 public Transform[] children;

     void Awake () {
         // Get the current sprite with an unscaled size
         sprite = GetComponent<SpriteRenderer>();
         
         // Generate a child prefab of the sprite renderer
         GameObject childPrefab = new GameObject();
		 childPrefab.transform.localScale = new Vector3(tileScalex, 1, 1);
         childSprite = childPrefab.AddComponent<SpriteRenderer>();
		 childSprite.color = sprite.color;
		 Vector3 targetposition = transform.position - childSprite.bounds.size.x*1/4 * Vector3.right;
         childPrefab.transform.position = targetposition;
         childSprite.sprite = spriteImg;
		 childSprite.sortingOrder = sprite.sortingOrder;
		 childSprite.sortingLayerName = sprite.sortingLayerName;

         // Loop through and spit out repeated tiles
         GameObject child;
		//  ntiles = 6;//(int)Mathf.Ceil(sprite.bounds.size.x/childSprite.bounds.size.x) + 2;
		 children = new Transform[ntiles];
		 children[0] = childPrefab.transform; 
		//  (int)Mathf.Round(sprite.bounds.size.y)
         for (int i = 1; i < ntiles; i++) {
             child = Instantiate(childPrefab) as GameObject;
             child.transform.parent = transform;
             child.transform.position = targetposition + (new Vector3(childSprite.bounds.size.x / transform.localScale.x, 0, 0) * i);
			 children[i] = child.transform;
         }
		 
		 lastChildIndex = ntiles - 1;
	 	 childSpriteSizeX = childSprite.bounds.size.x;
	 	//  childSpriteSizeY = childSprite.bounds.size.y;

         // Set the parent last on the prefab to prevent transform displacement
         childPrefab.transform.parent = transform;
 
         // Disable the currently existing sprite component since its now a repeated image
         sprite.enabled = false;
     }

	 void MoveTiles()
	 {
		 int nextIndex = (lastChildIndex + 1)%ntiles;
		 children[nextIndex].position = children[lastChildIndex].position + Vector3.right * childSpriteSizeX;
		 lastChildIndex = nextIndex;
	 }

	float timeCount = 0;
	 void Update()
	 {
		 moveInterval = childSpriteSizeX/mainCamera.currentSpeed *moveIntervalFactor;
		 timeCount += 1;
		 
		 if(timeCount > moveInterval)
		 {
			MoveTiles();
			timeCount = 0;
		 }
	 }
 }