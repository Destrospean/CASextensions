using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using MonoPatcherLib;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.CAS.CAP;
using Sims3.UI.Hud;
using Sims3.UI.OnlineDating;

namespace SDM_CASextension
{
    [Plugin]
    public class Main 
    {
        [TypePatch(typeof(CAPTackCollarSheet))]
        public class CAPTackCollarSheetPatch : CAPTackCollarSheet
        {
            public CAPTackCollarSheetPatch(uint winHandle) : base(winHandle)
            {
            }

            public override void Init()
            {
                mTopState = Responder.Instance.CASModel.CASMode == CASMode.Tack ? CASTopState.Tack : CASTopState.Collar;
                CASAgeGenderFlags species = Responder.Instance.CASModel.Species;
                WindowBase childByID;
                if (species != CASAgeGenderFlags.Horse)
                {
                    if (species != CASAgeGenderFlags.Cat)
                    {
                        childByID = GetChildByID(88908578, true);
                    }
                    else
                    {
                        childByID = GetChildByID(88908577, true);
                    }
                }
                else
                {
                    childByID = GetChildByID(88908576, true);
                }
                childByID.Visible = true;
                for (uint i = 0; i < 2; i++)
                {
                    (mButtons[(int)(UIntPtr)i] = (Button)childByID.GetChildByID(88908321 + i, true)).Click += OnNavigationButtonClick;
                    mButtons[(int)(UIntPtr)i].FocusAcquired += OnNavigationFocusAcquired;
                    mButtons[(int)(UIntPtr)i].FocusLost += OnNavigationFocusLost;
                    mButtons[(int)(UIntPtr)i].MouseDown += OnNavigationButtonMouseDown;
                    mButtons[(int)(UIntPtr)i].Tag = mButtons[(int)(UIntPtr)i].TooltipText;
                    mButtons[(int)(UIntPtr)i].TooltipText = "";
                    mButtonText[(int)(UIntPtr)i] = (Text)GetChildByID(88908305 + i, true);
                }
                mBaseColor = mButtonText[0].TextColor.ARGB;
                (mTextWindow = (Window)GetChildByID(88908304, true)).EffectFinished += OnTextFadeFinished;
                foreach (object effect in mTextWindow.EffectList)
                {
                    if ((mFadeEffect = effect as FadeEffect) != null)
                    {
                        mFadeTime = mFadeEffect.Duration;
                        break;
                    }
                }
                (mInputBlocker = GetChildByID(88908290, true)).Visible = false;
                Window window = (Window)GetChildByID(88908289, true);
                window.Visible = true;
                window.EffectFinished += OnPanelGlideFinished;
                foreach (object effect in window.EffectList)
                {
                    if ((mGlideEffect = effect as GlideEffect) != null)
                    {
                        mGlideTime = mGlideEffect.Duration;
                        break;
                    }
                }
                FadeTransitionFinished += OnFadeFinished;
            }
        }

        // CASCLOTHINGCATEGORY - NOSE RINGS, PET BODIES
        [TypePatch(typeof(CASClothingCategory))]
        public class CASClothingCategoryPatch : CASClothingCategory
        {
            public CASClothingCategoryPatch(uint winHandle) : base(winHandle)
            {
                mModel = Responder.Instance.CASModel;
            }

            public override void Init()
            {
                mAgeGenderFlags = mModel.Species | mModel.Age | mModel.Gender;
                foreach (object effect in EffectList)
                {
                    if ((mFadeEffect = effect as FadeEffect) != null)
                    {
                        mFadeTime = mFadeEffect.Duration;
                        break;
                    }
                }
                mFading = true;
                FadeTransitionFinished += OnFadeFinished;
                mInvalidCASPart.Key = ResourceKey.kInvalidResourceKey;
                mCategoryText = (Text)GetChildByID(98291457, true);
                (mClothingTypesGrid = (ItemGrid)GetChildByID(98291489, true)).InternalGrid.DrawPastLastRow = false;
                (mDesignButton = (Button)GetChildByID(98291482, true)).Click += OnDesignButtonClick;
                (mSaveButton = (Button)GetChildByID(98285828, true)).Click += OnSaveButtonClick;
                (mShareButton = (Button)GetChildByID(98291479, true)).Click += OnShareButtonClick;
                (mSortButton = (Button)GetChildByID(98291481, true)).Click += OnSortButtonClick;
                (mTrashButton = (Button)GetChildByID(98285829, true)).Click += OnTrashButtonClick;
                mSaveButton.Enabled = false;
                mShareButton.Enabled = false;
                mTrashButton.Enabled = false;
                mShareButton.MouseMove += CASController.TriggerParentGlowEffect;
                mShareButton.FocusLost += CASController.ReverseParentGlowEffect;
                mTrashButton.MouseMove += CASController.TriggerParentGlowEffect;
                mTrashButton.FocusLost += CASController.ReverseParentGlowEffect;
                ((Button)GetChildByID(98291478, true)).Click += OnButtonRandomizeClick;
                (mAccessoriesButton = (Button)(mCategoryButtonHolder = (Window)GetChildByID(98291472, true)).GetChildByID(98291600, true)).Click += OnCategoryButtonClick;
                (mBottomsButton = (Button)mCategoryButtonHolder.GetChildByID(98291474, true)).Click += OnCategoryButtonClick;
                (mHorseBridlesButton = (Button)mCategoryButtonHolder.GetChildByID(98291483, true)).Click += OnCategoryButtonClick;
                (mHorseSaddleButton = (Button)mCategoryButtonHolder.GetChildByID(98291484, true)).Click += OnCategoryButtonClick;
                (mOutfitsButton = (Button)mCategoryButtonHolder.GetChildByID(98291476, true)).Click += OnCategoryButtonClick;
                (mShoesButton = (Button)mCategoryButtonHolder.GetChildByID(98291475, true)).Click += OnCategoryButtonClick;
                (mTopsButton = (Button)mCategoryButtonHolder.GetChildByID(98291473, true)).Click += OnCategoryButtonClick;
                mPartsList = new ArrayList();
                mPartPresetsList = new ArrayList();
                if (mbIsHuman = mModel.Species == CASAgeGenderFlags.Human || mModel.Species == CASAgeGenderFlags.None)
                {
                    ((IconDrawable)((MultiDrawable)mAccessoriesButton.Drawable)[1]).Image = UIManager.LoadUIImage(ResourceKey.CreatePNGKey("cas_clothing_i_acc_r2", 0));
                }
                if (mbIsHuman)
                {
                    mAccessoryCategories = new ArrayList
                        {
                            BodyTypes.Glasses,
                            BodyTypes.Earrings,
                            BodyTypes.NoseRing,                                                                     // Added: Nose Rings
                            BodyTypes.LeftEarring,
                            BodyTypes.RightEarring,
                            BodyTypes.Necklace,
                            BodyTypes.Armband,
                            BodyTypes.Bracelet,
                            BodyTypes.Gloves,
                            BodyTypes.Ring,
                            BodyTypes.Socks,
                            BodyTypes.Dental,
                            BodyTypes.LeftGarter,
                            BodyTypes.RightGarter,
                            BodyTypes.BirthMark
                        };
                    if (mModel.CurrentOccultType == OccultTypes.Mermaid && mModel.HiddenInCAS)
                    {
                        mAccessoryCategories.Remove(BodyTypes.Socks);
                    }
                }
                else if (mModel.Species == CASAgeGenderFlags.Horse || mModel.Species == CASAgeGenderFlags.Dog || mModel.Species == CASAgeGenderFlags.LittleDog || mModel.Species == CASAgeGenderFlags.Cat)
                {
                    mAccessoryCategories = new ArrayList
                        {
                            BodyTypes.PetBreastCollar,
                            BodyTypes.PetBlanket,
                            BodyTypes.PetBody                                                                       // Added: Pet Bodies
                        };
                }
                if ((mContentTypeFilter = CASPuck.GetContentTypeFilter()) != null)
                {
                    mContentTypeFilter.Tag = mSortButton;
                    CASPuck.SetContentFilterPosition(GetChildByID(98291488, true));
                    mContentTypeFilter.FiltersChanged += PopulateTypesGrid;
                }
                mModel.UndoSelected += OnUndoRedo;
                mModel.RedoSelected += OnUndoRedo;
                mModel.OnCASPartAdded += OnCASPartAddedRemoved;
                mModel.OnCASPartRemoved += OnCASPartAddedRemoved;
                Responder.Instance.StoreUI.OnFeaturedItemRemoveError += OnFeaturedItemRemoveErrorHandler;
                Tick += OnTick;
            }
        }

        // CASMAKEUP STUFF - MASCARA, FACES, SCALPS
        [TypePatch(typeof(CASMakeup))]
        public class CASFacialBlendPanelPatch : CASFacialBlendPanel
        {
            public new enum ControlIDs : uint
            {
                // Token: 0x0400305E RID: 12382
                ItemThumbnailWindow = 32,
                // Token: 0x0400305F RID: 12383
                ItemCustom = 35,
                // Token: 0x04003060 RID: 12384
                ItemWardrobe = 41,
                // Token: 0x04003061 RID: 12385
                ItemThumbnailColor1 = 48,
                // Token: 0x04003062 RID: 12386
                ItemThumbnailColor2,
                // Token: 0x04003063 RID: 12387
                ItemThumbnailColor3,
                // Token: 0x04003064 RID: 12388
                ItemThumbnailColor4,
                // Token: 0x04003065 RID: 12389
                ButtonEyeshadow = 256,
                // Token: 0x04003066 RID: 12390
                ButtonEyeliner,
                // Token: 0x04003067 RID: 12391
                ButtonBlush,
                // Token: 0x04003068 RID: 12392
                ButtonLipstick,

                ButtonMascara = 262,
                // Token: 0x04003069 RID: 12393
                ButtonCostume,
                // Token: 0x0400306A RID: 12394
                WindowMakeup = 4096,
                // Token: 0x0400306B RID: 12395
                GridMakeupParts,
                // Token: 0x0400306C RID: 12396
                GridMakeupPresets,
                // Token: 0x0400306D RID: 12397
                ButtonDeleteCustom,
                // Token: 0x0400306E RID: 12398
                ButtonShareCustom,
                // Token: 0x0400306F RID: 12399
                ButtonColor,
                // Token: 0x04003070 RID: 12400
                ButtonFilter,
                // Token: 0x04003071 RID: 12401
                WindowCostume = 8192,
                // Token: 0x04003072 RID: 12402
                GridCostumeParts,
                // Token: 0x04003073 RID: 12403
                ButtonDeleteCostume = 8195,
                // Token: 0x04003074 RID: 12404
                ButtonShareCostume,
                // Token: 0x04003075 RID: 12405
                ButtonDesignCostume,
                // Token: 0x04003076 RID: 12406
                ButtonCostumeFilter,
                // Token: 0x04003077 RID: 12407
                ContentTypeFilterPositionHolder,
                // Token: 0x04003078 RID: 12408
                LockCategoryButton = 215017952,
                // Token: 0x04003079 RID: 12409
                SliderOpacity = 158168880
            }

            public CASFacialBlendPanelPatch(uint winHandle) : base(winHandle)
            {
            }

            void SetCategory(BodyTypes category)
            {
                CASMakeup casMakeup = (CASMakeup)(object)this;
                casMakeup.mWindowCostume.Visible = false;
                casMakeup.mWindowMakeup.Visible = false;
                CASFacialDetails casFacialDetails = CASFacialDetails.gSingleton;
                switch (category)
                {
                    case BodyTypes.FirstFace:
                    case BodyTypes.EyeShadow:
                    case BodyTypes.Mascara:
                    case BodyTypes.Face:
                    case BodyTypes.Scalp:
                    case BodyTypes.EyeLiner:
                    case BodyTypes.Blush:
                        casMakeup.mWindowMakeup.Visible = true;
                        casFacialDetails.SetLongPanel(true);
                        Tick -= casMakeup.OnTick;
                        casMakeup.mButtonCostumeFilter.Selected = false;
                        casMakeup.mContentTypeFilter.Visible = false;
                        break;
                    case BodyTypes.CostumeMakeup:
                        casMakeup.mWindowCostume.Visible = true;
                        casFacialDetails.SetLongPanel(false);
                        casFacialDetails.SetShortPanelHeight(570);
                        if (casMakeup.GetWornPart(category).Key != casMakeup.kInvalidCASPart.Key)
                        {
                            casMakeup.mButtonDesignCostume.Enabled = true;
                        }
                        else
                        {
                            casMakeup.mButtonDesignCostume.Enabled = false;
                        }
                        casMakeup.UpdateCostumePresetState();
                        Tick -= casMakeup.OnTick;
                        Tick += casMakeup.OnTick;
                        break;
                }
                CASMakeup.sCategory = category;
                HideUnusedIcons();
                casMakeup.PopulatePartsGrid(CASMakeup.sCategory);
                casMakeup.PopulatePresetsGrid(CASMakeup.sCategory, casMakeup.GetWornPart(CASMakeup.sCategory), casMakeup.mButtonFilter.Selected);
            }

            public void HideUnusedIcons()
            {
                List<Button> buttons = new List<Button>();
                foreach (BodyTypes bodyType in new[]
                    {
                        BodyTypes.EyeShadow,
                        BodyTypes.EyeLiner,
                        BodyTypes.Blush,
                        BodyTypes.Mascara,
                        BodyTypes.FirstFace,
                        BodyTypes.CostumeMakeup
                    })
                {
                    ArrayList visibleCASParts = Responder.Instance.CASModel.GetVisibleCASParts(bodyType);
                    CASMakeup.ControlIDs id = CASMakeup.ControlIDs.ButtonCostume;
                    switch (bodyType)
                    {
                        case BodyTypes.FirstFace:
                            id = CASMakeup.ControlIDs.ButtonLipstick;
                            break;
                        case BodyTypes.EyeShadow:
                            id = CASMakeup.ControlIDs.ButtonEyeshadow;
                            break;
                        case BodyTypes.Mascara:
                            id = (CASMakeup.ControlIDs)262;
                            break;
                        case BodyTypes.EyeLiner:
                            id = CASMakeup.ControlIDs.ButtonEyeliner;
                            break;
                        case BodyTypes.Blush:
                            id = CASMakeup.ControlIDs.ButtonBlush;
                            break;
                    }
                    Button button = (Button)GetChildByID((uint)id, true);
                    if (visibleCASParts.Count > 0)
                    {
                        button.Visible = true;
                        buttons.Add(button);
                    }
                    else
                    {
                        button.Visible = false;
                        if (button.Selected)
                        {
                            button.Selected = false;
                            ((Button)GetChildByID(260, true)).Selected = true;
                            if (CASMakeup.sCategory != BodyTypes.CostumeMakeup)
                            {
                                SetCategory(BodyTypes.CostumeMakeup);
                            }
                            return;
                        }
                    }
                }
                float x = 110 - buttons.Count * .5f * 42 - (buttons.Count - 1) * .5f * -3;
                foreach (Button button in buttons)
                {
                    button.Position = new Vector2(x, button.Position.y);
                    x += 39;
                }
            }

            public override void Init()
            {
                CASMakeup casMakeup = (CASMakeup)(object)this;
                foreach (object effect in EffectList)
                {
                    if ((casMakeup.mFadeEffect = effect as FadeEffect) != null)
                    {
                        casMakeup.mFadeTime = casMakeup.mFadeEffect.Duration;
                        break;
                    }
                }
                FadeTransitionFinished += casMakeup.OnFadeFinished;
                Init();
                CASMakeup.sCategory = BodyTypes.EyeShadow;
                Button button = (Button)GetChildByID(256, true);
                button.Selected = CASMakeup.sCategory == BodyTypes.EyeShadow;
                button.Click += OnButtonTabClick;
                (button = (Button)GetChildByID(257, true)).Selected = CASMakeup.sCategory == BodyTypes.EyeLiner;
                button.Click += OnButtonTabClick;
                (button = (Button)GetChildByID(262, true)).Selected = CASMakeup.sCategory == BodyTypes.Mascara;     // Added: Mascara
                button.Click += OnButtonTabClick;
                (button = (Button)GetChildByID(258, true)).Selected = CASMakeup.sCategory == BodyTypes.Blush;
                button.Click += OnButtonTabClick;
                (button = (Button)GetChildByID(259, true)).Selected = CASMakeup.sCategory == BodyTypes.FirstFace;
                button.Click += OnButtonTabClick;
                (button = (Button)GetChildByID(263, true)).Selected = CASMakeup.sCategory == BodyTypes.Face;        // Added: Faces
                button.Click += OnButtonTabClick;
                (button = (Button)GetChildByID(264, true)).Selected = CASMakeup.sCategory == BodyTypes.Scalp;       // Added: Scalps
                button.Click += OnButtonTabClick;
                (button = (Button)GetChildByID(260, true)).Selected = CASMakeup.sCategory == BodyTypes.CostumeMakeup;
                button.Click += OnButtonTabClick;
                (button = (Button)GetChildByID(215017952, true)).Selected = Responder.Instance.CASModel.PropagateMakeUpStyles;
                Responder.Instance.CASModel.PropagateMakeUpStyles = button.Selected;
                if (button.Selected)
                {
                    button.TooltipText = Responder.Instance.LocalizationModel.LocalizeString("Ui/Caption/CAS:ApplyToAllMakeup");
                }
                else
                {
                    button.TooltipText = Responder.Instance.LocalizationModel.LocalizeString("Ui/Caption/CAS:PerCategoryMakeup");
                }
                button.Click += casMakeup.OnLockCategoryButtonClick;
                (casMakeup.mGridMakeupParts = (ItemGrid)(casMakeup.mWindowMakeup = (Window)GetChildByID(4096, true)).GetChildByID(4097, true)).ItemClicked += casMakeup.OnGridPartsClick;
                (casMakeup.mGridMakeupPresets = (ItemGrid)casMakeup.mWindowMakeup.GetChildByID(4098, true)).ItemClicked += casMakeup.OnGridPresetsClick;
                (casMakeup.mButtonFilter = (Button)casMakeup.mWindowMakeup.GetChildByID(4102, true)).Click += casMakeup.OnButtonFilterClick;
                (casMakeup.mButtonCostumeFilter = (Button)(casMakeup.mWindowCostume = (Window)GetChildByID(8192, true)).GetChildByID(8198, true)).Click += casMakeup.OnButtonCostumeFilterClick;
                casMakeup.mGridCostumeParts = (ItemGrid)casMakeup.mWindowCostume.GetChildByID(8193, true);
                (casMakeup.mOpacitySlider = (Slider)GetChildByID(158168880, true)).MouseUp += casMakeup.OnOpacitySliderMouseUp;
                casMakeup.mOpacitySlider.SliderValueChange += casMakeup.OnOpacitySliderChange;
                if ((casMakeup.mContentTypeFilter = CASPuck.GetContentTypeFilter()) != null)
                {
                    casMakeup.mContentTypeFilter.Tag = casMakeup.mButtonCostumeFilter;
                    CASPuck.SetContentFilterPosition(GetChildByID(8199, true));
                    casMakeup.mContentTypeFilter.FiltersChanged += casMakeup.OnFilterChanged;
                }
                (casMakeup.mButtonColor = (Button)casMakeup.mWindowMakeup.GetChildByID(4101, true)).Click += casMakeup.OnButtonDesignClick;
                (casMakeup.mButtonShare = (Button)casMakeup.mWindowMakeup.GetChildByID(4100, true)).Click += casMakeup.OnButtonShareClick;
                (casMakeup.mButtonDelete = (Button)casMakeup.mWindowMakeup.GetChildByID(4099, true)).Click += casMakeup.OnButtonDeleteClick;
                (casMakeup.mButtonDeleteCostume = (Button)GetChildByID(8195, true)).Click += casMakeup.OnButtonDeleteClick;
                (casMakeup.mButtonShareCostume = (Button)GetChildByID(8196, true)).Click += casMakeup.OnButtonShareClick;
                (casMakeup.mButtonDesignCostume = (Button)GetChildByID(8197, true)).Click += casMakeup.OnButtonDesignClick;
                ICASModel casModel = Responder.Instance.CASModel;
                casModel.OnCASPartAdded += casMakeup.OnPartAdded;
                casModel.OnCASPartRemoved += casMakeup.OnPartRemoved;
                casModel.UndoSelected += casMakeup.OnUndoRedo;
                casModel.RedoSelected += casMakeup.OnUndoRedo;
                SetCategory(CASMakeup.sCategory);
            }

            public void OnButtonTabClick(WindowBase sender, UIButtonClickEventArgs args)
            {
                switch (sender.ID)
                {
                    case 256:
                        SetCategory(BodyTypes.EyeShadow);
                        return;
                    case 257:
                        SetCategory(BodyTypes.EyeLiner);
                        return;
                    case 258:
                        SetCategory(BodyTypes.Blush);
                        return;
                    case 259:
                        SetCategory(BodyTypes.FirstFace);
                        return;
                    case 260:
                        SetCategory(BodyTypes.CostumeMakeup);
                        return;
                    case 262:
                        SetCategory(BodyTypes.Mascara);
                        return;
                    case 263:
                        SetCategory(BodyTypes.Face);
                        return;
                    case 264:
                        SetCategory(BodyTypes.Scalp);
                        return;
                }
            }
        }
    }
}
