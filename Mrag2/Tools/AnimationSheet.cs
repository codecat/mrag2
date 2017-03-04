using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Mrag2.XML;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mrag2.Extensions;

namespace Mrag2
{
	public class AnimationState
	{
		public int iCurrentFrame;
		public double fCurrentFrameCounter;
		public bool bPaused;

		public AnimationState()
		{
			Reset();
		}

		public void Reset()
		{
			iCurrentFrame = 0;
			fCurrentFrameCounter = 0;
			bPaused = false;
		}
	}

	public class AnimationFrame
	{
		public Vector2 anf_vOffset;
		public double anf_fTime;
		public bool anf_bPause;

		public AnimationFrame()
		{
			anf_fTime = 1;
			anf_bPause = false;
		}

		public void Load(XmlTag tagFrame)
		{
			if (tagFrame.Attributes.ContainsKey("offset")) { anf_vOffset = tagFrame.Attributes["offset"].ParseVector2(); }
			if (tagFrame.Attributes.ContainsKey("time")) { anf_fTime = double.Parse(tagFrame.Attributes["time"]); }
			if (tagFrame.Attributes.ContainsKey("pause")) { anf_bPause = Boolean.Parse(tagFrame.Attributes["pause"]); }
		}
	}

	public class Animation
	{
		public AnimationSheet ani_pSheet;

		public List<AnimationFrame> ani_saFrames;
		public string ani_strName;

		public Vector2 ani_vSize;
		public Rectangle ani_rectOffset;

		public bool ani_bResetOnActive = false;
		public bool ani_bLooping = true;
		int ani_iStartFrame = 0;
		int ani_iLoopFrame = 0;

		public Animation ani_pAliasOwner = null;
		public bool ani_bFlipH = false;
		public bool ani_bFlipV = false;

		public int ani_iHue = 0;

		public AnimationState ani_state = new AnimationState();

		public Animation()
		{
			ani_saFrames = new List<AnimationFrame>();

			ani_bResetOnActive = false;
			ani_bLooping = true;
			ani_iStartFrame = 0;
			ani_iLoopFrame = 0;

			ani_pAliasOwner = null;
			ani_bFlipH = false;
			ani_bFlipV = false;
			ani_iHue = 0;
		}

		public void SetActive(AnimationState state)
		{
			Animation pAnim = this;
			if (ani_pAliasOwner != null) {
				pAnim = ani_pAliasOwner;
			}

			if (pAnim.ani_bResetOnActive) {
				ani_state.iCurrentFrame = ani_iStartFrame;
				ani_state.fCurrentFrameCounter = 0;
			}
		}

		public void AdvanceFrame() //This will also unpause the animation state
		{
			AdvanceFrame(ani_state);
		}

		public void AdvanceFrame(AnimationState state) //This will also unpause the animation state
		{
			Animation pAnim = this;
			if (ani_pAliasOwner != null) {
				pAnim = ani_pAliasOwner;
			}

			state.bPaused = false;
			state.fCurrentFrameCounter = 0;
			if (++state.iCurrentFrame >= pAnim.ani_saFrames.Count) {
				state.iCurrentFrame = pAnim.ani_iLoopFrame;
			}
		}

		public void Load(XmlTag tagAnim)
		{
			if (tagAnim.Attributes.ContainsKey("name")) { ani_strName = tagAnim.Attributes["name"]; }
			if (tagAnim.Attributes.ContainsKey("size")) { ani_vSize = tagAnim.Attributes["size"].ParseVector2(); }
			if (tagAnim.Attributes.ContainsKey("offset")) { ani_rectOffset = tagAnim.Attributes["offset"].ParseRectangle(); }
			if (tagAnim.Attributes.ContainsKey("resetonactive")) { ani_bResetOnActive = bool.Parse(tagAnim.Attributes["resetonactive"]); }
			if (tagAnim.Attributes.ContainsKey("looping")) { ani_bLooping = bool.Parse(tagAnim.Attributes["looping"]); }
			if (tagAnim.Attributes.ContainsKey("startframe")) { ani_iStartFrame = Convert.ToInt32(tagAnim.Attributes["startframe"]); }
			if (tagAnim.Attributes.ContainsKey("loopframe")) { ani_iLoopFrame = Convert.ToInt32(tagAnim.Attributes["loopframe"]); }
			if (tagAnim.Attributes.ContainsKey("fliph")) { ani_bFlipH = bool.Parse(tagAnim.Attributes["fliph"]); }
			if (tagAnim.Attributes.ContainsKey("flipv")) { ani_bFlipV = bool.Parse(tagAnim.Attributes["flipv"]); }

			ani_state.iCurrentFrame = ani_iStartFrame;

			XmlTag[] xmlFrames;
			xmlFrames = tagAnim.FindTagsByName("frame");

			for (int i = 0; i < xmlFrames.Length; i++) {
				XmlTag tag = xmlFrames[i];
				AnimationFrame frame = new AnimationFrame();
				frame.Load(tag);
				ani_saFrames.Add(frame);
			}

			XmlTag[] xmlAliases;
			xmlAliases = tagAnim.FindTagsByName("alias");

			for (int i = 0; i < xmlAliases.Length; i++) {
				XmlTag tag = xmlAliases[i];
				Animation anim = new Animation();
				anim.ani_strName = tag.Attributes["name"];
				anim.ani_pAliasOwner = this;
				if (tag.Attributes.ContainsKey("fliph")) { anim.ani_bFlipH = bool.Parse(tag.Attributes["fliph"]); }
				if (tag.Attributes.ContainsKey("flipy")) { anim.ani_bFlipH = bool.Parse(tag.Attributes["flipy"]); }
				if (tag.Attributes.ContainsKey("hue")) { anim.ani_iHue = Convert.ToInt32(tag.Attributes["hue"]); }
				ani_pSheet.ans_saAnimations.Add(anim);
			}
		}

		public Vector2 Render(GameTime gameTime, Vector2 vPos, AnimationState state, float fRotation, float fOverX, float fOverY, float fDepth)
		{
			Animation pAnim = this;
			if (ani_pAliasOwner != null) {
				pAnim = ani_pAliasOwner;
			}
			return Render(gameTime, vPos, pAnim.ani_vSize, state, fRotation, fOverX, fOverY, Color.White, fDepth);
		}

		public Vector2 Render(GameTime gameTime, Vector2 vPos, AnimationState state, float fRotation, float fOverX, float fOverY, Color color, float fDepth)
		{
			Animation pAnim = this;
			if (ani_pAliasOwner != null) {
				pAnim = ani_pAliasOwner;
			}
			return Render(gameTime, vPos, pAnim.ani_vSize, state, fRotation, fOverX, fOverY, color, fDepth);
		}

		public Vector2 Render(GameTime gameTime, Vector2 vPos, Vector2 vSize, AnimationState state, float fRotation, float fOverX, float fOverY, Color color, float fDepth)
		{
			Animation pAnim = this;
			if (ani_pAliasOwner != null) {
				pAnim = ani_pAliasOwner;
			}

			CustomSpriteBatch rend = pAnim.ani_pSheet.ans_pSpriteBatch;
			Texture2D tex = pAnim.ani_pSheet.ans_pTexture;

			if (state.iCurrentFrame >= pAnim.ani_saFrames.Count) {
				state.Reset();
			}

			AnimationFrame frame = pAnim.ani_saFrames[state.iCurrentFrame];

			RotRect rectDest = new RotRect();
			rectDest.width = vSize.X;
			rectDest.height = vSize.Y;

			float fOffsetX = pAnim.ani_rectOffset.X;
			float fOffsetY = pAnim.ani_rectOffset.Y;

			float fWidth = pAnim.ani_rectOffset.Width;
			if (fWidth == 0) {
				fWidth = vSize.X;
			}
			if (ani_bFlipH) {
				fOffsetX -= ((fOffsetX - rectDest.width / 2) * 2) + fWidth;
			}

			float fHeight = pAnim.ani_rectOffset.Height;
			if (fHeight == 0) {
				fHeight = vSize.Y;
			}
			if (ani_bFlipV) {
				fOffsetY -= ((fOffsetY - rectDest.height / 2) * 2) + fHeight;
			}

			rectDest.x = vPos.X - fOffsetX;
			rectDest.y = vPos.Y - fOffsetY;

			Rectangle rectSrc = new Rectangle();
			rectSrc.X = (int)frame.anf_vOffset.X;
			rectSrc.Y = (int)frame.anf_vOffset.Y;
			rectSrc.Width = (int)pAnim.ani_vSize.X;
			rectSrc.Height = (int)pAnim.ani_vSize.Y;

			if (fOverX != 0) {
				rectDest.width = fOverX;
			}
			if (fOverY != 0) {
				rectDest.height = fOverY;
			}

			rend.CurrentDepth = fDepth;
			rend.Draw(tex, rectSrc, rectDest, color, ani_bFlipH, ani_bFlipV, false);

			if (!frame.anf_bPause) {
				state.fCurrentFrameCounter += gameTime.ElapsedGameTime.TotalMilliseconds;

				if (state.fCurrentFrameCounter++ >= frame.anf_fTime) {
					AdvanceFrame(state);
				}
			}

			// hmm..
			if (fOverX != 0) {
				fWidth = fOverX;
			}
			if (fOverY != 0) {
				fHeight = fOverY;
			}

			return new Vector2((int)fWidth, (int)fHeight);
		}
	}

	public class AnimationSheet
	{
		public List<Animation> ans_saAnimations;
		public string ans_strName;
		public string ans_strTextureFilename;
		public Texture2D ans_pTexture;
		public Dictionary<int, Texture2D> ans_saHues;
		public CustomSpriteBatch ans_pSpriteBatch;

		public string ans_strLatestAnimation;

		public AnimationSheet()
		{
			ans_saAnimations = new List<Animation>();
			ans_pTexture = null;
			ans_pSpriteBatch = null;
		}

		public AnimationSheet(CustomSpriteBatch spriteBatch, string strFilename)
		{
			ans_saAnimations = new List<Animation>();
			Load(spriteBatch, strFilename);
		}

		public Animation GetAnimation(string strName)
		{
			for (int i = 0; i < ans_saAnimations.Count; i++) {
				Animation anim = ans_saAnimations[i];
				if (anim.ani_strName == strName) {
					return anim;
				}
			}
			return null;
		}

		public void Load(CustomSpriteBatch spriteBatch, string strFilename)
		{
			XmlFile xmlFile = new XmlFile(strFilename.Replace(".png", ".xml"));
			Load(spriteBatch, xmlFile.Root.FindTagByName("sheet"), strFilename);
		}

		public void Load(CustomSpriteBatch spriteBatch, XmlTag xmlSheet, string strTextureFilename)
		{
			Debug.Assert(ans_pTexture == null);

			ans_pSpriteBatch = spriteBatch;
			ans_strTextureFilename = strTextureFilename;
			ans_pTexture = ContentRegister.Texture(strTextureFilename);

			if (xmlSheet.Attributes.ContainsKey("name")) { ans_strName = xmlSheet.Attributes["name"]; }

			XmlTag[] xmlAnimations;
			xmlAnimations = xmlSheet.FindTagsByName("anim");

			for (int i = 0; i < xmlAnimations.Length; i++) {
				XmlTag tag = xmlAnimations[i];
				Animation anim = new Animation();
				anim.ani_pSheet = this;
				anim.Load(tag);
				ans_saAnimations.Add(anim);
			}
		}

		public void Unload()
		{
			if (ans_pTexture != null) {
				ans_pTexture.Dispose();
				ans_pTexture = null;
			}
			ans_saHues.Clear();
		}

		public Vector2 Render(GameTime gameTime, string strName, Vector2 vPos, float fRotation, float fOverX, float fOverY, float fDepth = 0)
		{
			Animation pAnim = GetAnimation(strName);
			Debug.Assert(pAnim != null);
			if (pAnim == null) {
				return new Vector2(0, 0);
			}

			if (ans_strLatestAnimation != strName) {
				ans_strLatestAnimation = strName;
				pAnim.SetActive(pAnim.ani_state);
			}

			return pAnim.Render(gameTime, vPos, pAnim.ani_state, fRotation, fOverX, fOverY, fDepth);
		}

		public Vector2 Render(GameTime gameTime, string strName, Vector2 vPos, AnimationState state, float fRotation, float fOverX, float fOverY, float fDepth = 0)
		{
			Animation pAnim = GetAnimation(strName);
			Debug.Assert(pAnim != null);
			if (pAnim == null) {
				return new Vector2(0, 0);
			}

			if (ans_strLatestAnimation != strName) {
				ans_strLatestAnimation = strName;
				pAnim.SetActive(state);
			}

			return pAnim.Render(gameTime, vPos, state, fRotation, fOverX, fOverY, fDepth);
		}

		public Vector2 Render(GameTime gameTime, string strName, Vector2 vPos, Vector2 vSize, float fRotation, float fOverX, float fOverY, float fDepth = 0)
		{
			Animation pAnim = GetAnimation(strName);
			Debug.Assert(pAnim == null);
			if (pAnim == null) {
				return new Vector2(0, 0);
			}

			if (ans_strLatestAnimation != strName) {
				ans_strLatestAnimation = strName;
				pAnim.SetActive(pAnim.ani_state);
			}

			return pAnim.Render(gameTime, vPos, vSize, pAnim.ani_state, fRotation, fOverX, fOverY, Color.White, fDepth);
		}

		public Vector2 Render(GameTime gameTime, string strName, Vector2 vPos, Vector2 vSize, AnimationState state, float fRotation, float fOverX, float fOverY, float fDepth = 0)
		{
			Animation pAnim = GetAnimation(strName);
			Debug.Assert(pAnim != null);
			if (pAnim == null) {
				return new Vector2(0, 0);
			}

			if (ans_strLatestAnimation != strName) {
				ans_strLatestAnimation = strName;
				pAnim.SetActive(state);
			}

			return pAnim.Render(gameTime, vPos, vSize, state, fRotation, fOverX, fOverY, Color.White, fDepth);
		}
	}

	public class AnimationSheetCollection
	{
		public List<AnimationSheet> asc_saSheets = new List<AnimationSheet>();

		public AnimationSheetCollection(CustomSpriteBatch spriteBatch, string strFilename)
		{
			Load(spriteBatch, strFilename);
		}

		public void Load(CustomSpriteBatch spriteBatch, string strFilename)
		{
			Unload();

			XmlFile xmlFile = new XmlFile(strFilename.Replace(".png", ".xml"));
			XmlTag pSheets = xmlFile.Root.FindTagByName("sheets");
			Debug.Assert(pSheets != null);
			if (pSheets == null) {
				return;
			}

			XmlTag[] sheets = pSheets.FindTagsByName("sheet");

			for (int i = 0; i < sheets.Length; i++) {
				AnimationSheet sheet = new AnimationSheet();
				sheet.Load(spriteBatch, sheets[i], strFilename);
				asc_saSheets.Add(sheet);
			}
		}

		public void Unload()
		{
			asc_saSheets.Clear();
		}

		public AnimationSheet GetSheet(string strName)
		{
			for (int i = 0; i < asc_saSheets.Count; i++) {
				if (asc_saSheets[i].ans_strName == strName) {
					return asc_saSheets[i];
				}
			}
			return null;
		}

		public Vector2 Render(GameTime gameTime, string strSheetName, string strAnimationName, Vector2 vPos, float fRotation, float fOverX, float fOverY)
		{
			AnimationSheet pSheet = GetSheet(strSheetName);
			Debug.Assert(pSheet != null);
			if (pSheet == null) {
				return new Vector2(0, 0);
			}

			return pSheet.Render(gameTime, strAnimationName, vPos, fRotation, fOverX, fOverY);
		}
		public Vector2 Render(GameTime gameTime, string strSheetName, string strAnimationName, Vector2 vPos, AnimationState state, float fRotation, float fOverX, float fOverY)
		{
			AnimationSheet pSheet = GetSheet(strSheetName);
			Debug.Assert(pSheet != null);
			if (pSheet == null) {
				return new Vector2(0, 0);
			}

			return pSheet.Render(gameTime, strAnimationName, vPos, state, fRotation, fOverX, fOverY);
		}
		public Vector2 Render(GameTime gameTime, string strSheetName, string strAnimationName, Vector2 vPos, Vector2 vSize, float fRotation, float fOverX, float fOverY)
		{
			AnimationSheet pSheet = GetSheet(strSheetName);
			Debug.Assert(pSheet != null);
			if (pSheet == null) {
				return new Vector2(0, 0);
			}

			return pSheet.Render(gameTime, strAnimationName, vPos, vSize, fRotation, fOverX, fOverY);
		}
		public Vector2 Render(GameTime gameTime, string strSheetName, string strAnimationName, Vector2 vPos, Vector2 vSize, AnimationState state, float fRotation, float fOverX, float fOverY)
		{
			AnimationSheet pSheet = GetSheet(strSheetName);
			Debug.Assert(pSheet != null);
			if (pSheet == null) {
				return new Vector2(0, 0);
			}

			return pSheet.Render(gameTime, strAnimationName, vPos, vSize, state, fRotation, fOverX, fOverY);
		}

		public Vector2 GetSize(string strSheetName, string strAnimationName)
		{
			AnimationSheet pSheet = GetSheet(strSheetName);
			Debug.Assert(pSheet != null);
			if (pSheet == null) {
				return new Vector2(0, 0);
			}

			Animation pAnim = pSheet.GetAnimation(strAnimationName);
			Debug.Assert(pAnim != null);
			if (pAnim == null) {
				return new Vector2(0, 0);
			}

			return pAnim.ani_vSize;
		}

		public AnimationSheet this[string strName]
		{
			get { return GetSheet(strName); }
		}
	}
}
