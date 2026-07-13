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
                    mButtons[(int)(UIntPtr)i] = childByID.GetChildByID(88908321 + i, true) as Button;
                    mButtons[(int)(UIntPtr)i].Click += OnNavigationButtonClick;
                    mButtons[(int)(UIntPtr)i].FocusAcquired += OnNavigationFocusAcquired;
                    mButtons[(int)(UIntPtr)i].MouseDown += OnNavigationButtonMouseDown;
                    mButtons[(int)(UIntPtr)i].FocusLost += OnNavigationFocusLost;
                    mButtons[(int)(UIntPtr)i].Tag = mButtons[(int)(UIntPtr)i].TooltipText;
                    mButtons[(int)(UIntPtr)i].TooltipText = "";
                    mButtonText[(int)(UIntPtr)i] = GetChildByID(88908305 + i, true) as Text;
                }
                mBaseColor = mButtonText[0].TextColor.ARGB;
                mTextWindow = GetChildByID(88908304, true) as Window;
                mTextWindow.EffectFinished += OnTextFadeFinished;
                foreach (object effect in mTextWindow.EffectList)
                {
                    mFadeEffect = effect as FadeEffect;
                    if (mFadeEffect != null)
                    {
                        mFadeTime = mFadeEffect.Duration;
                        break;
                    }
                }
                mInputBlocker = GetChildByID(88908290, true);
                mInputBlocker.Visible = false;
                Window window = GetChildByID(88908289, true) as Window;
                window.Visible = true;
                window.EffectFinished += OnPanelGlideFinished;
                foreach (object effect in window.EffectList)
                {
                    mGlideEffect = effect as GlideEffect;
                    if (mGlideEffect != null)
                    {
                        mGlideTime = mGlideEffect.Duration;
                        break;
                    }
                }
                FadeTransitionFinished += OnFadeFinished;
            }
        }

        // CASCLOTHINGCATEGORY - NOSERINGS, PET BODIES
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
                    mFadeEffect = effect as FadeEffect;
                    if (mFadeEffect != null)
                    {
                        mFadeTime = mFadeEffect.Duration;
                        break;
                    }
                }
                mFading = true;
                FadeTransitionFinished += OnFadeFinished;
                mInvalidCASPart.Key = ResourceKey.kInvalidResourceKey;
                mTrashButton = GetChildByID(98285829, true) as Button;
                mSaveButton = GetChildByID(98285828, true) as Button;
                mShareButton = GetChildByID(98291479, true) as Button;
                mSortButton = GetChildByID(98291481, true) as Button;
                mCategoryText = GetChildByID(98291457, true) as Text;
                mDesignButton = GetChildByID(98291482, true) as Button;
                mShareButton.Enabled = false;
                mTrashButton.Enabled = false;
                mSaveButton.Enabled = false;
                mClothingTypesGrid = GetChildByID(98291489, true) as ItemGrid;
                mClothingTypesGrid.InternalGrid.DrawPastLastRow = false;
                mTrashButton.Click += OnTrashButtonClick;
                mSaveButton.Click += OnSaveButtonClick;
                mShareButton.Click += OnShareButtonClick;
                mSortButton.Click += OnSortButtonClick;
                mDesignButton.Click += OnDesignButtonClick;
                mShareButton.MouseMove += CASController.TriggerParentGlowEffect;
                mShareButton.FocusLost += CASController.ReverseParentGlowEffect;
                mTrashButton.MouseMove += CASController.TriggerParentGlowEffect;
                mTrashButton.FocusLost += CASController.ReverseParentGlowEffect;
                Button button = GetChildByID(98291478, true) as Button;
                button.Click += OnButtonRandomizeClick;
                mCategoryButtonHolder = GetChildByID(98291472, true) as Window;
                mTopsButton = mCategoryButtonHolder.GetChildByID(98291473, true) as Button;
                mTopsButton.Click += OnCategoryButtonClick;
                mBottomsButton = mCategoryButtonHolder.GetChildByID(98291474, true) as Button;
                mBottomsButton.Click += OnCategoryButtonClick;
                mShoesButton = mCategoryButtonHolder.GetChildByID(98291475, true) as Button;
                mShoesButton.Click += OnCategoryButtonClick;
                mOutfitsButton = mCategoryButtonHolder.GetChildByID(98291476, true) as Button;
                mOutfitsButton.Click += OnCategoryButtonClick;
                mAccessoriesButton = mCategoryButtonHolder.GetChildByID(98291600, true) as Button;
                mAccessoriesButton.Click += OnCategoryButtonClick;
                mHorseBridlesButton = mCategoryButtonHolder.GetChildByID(98291483, true) as Button;
                mHorseBridlesButton.Click += OnCategoryButtonClick;
                mHorseSaddleButton = mCategoryButtonHolder.GetChildByID(98291484, true) as Button;
                mHorseSaddleButton.Click += OnCategoryButtonClick;
                mPartsList = new ArrayList();
                mPartPresetsList = new ArrayList();
                if (mbIsHuman = mModel.Species == CASAgeGenderFlags.Human || mModel.Species == CASAgeGenderFlags.None)
                {
                    MultiDrawable multiDrawable = mAccessoriesButton.Drawable as MultiDrawable;
                    IconDrawable iconDrawable = multiDrawable[1u] as IconDrawable;
                    iconDrawable.Image = UIManager.LoadUIImage(ResourceKey.CreatePNGKey("cas_clothing_i_acc_r2", 0));
                }
                mAccessoryCategories = new ArrayList();
                if (mbIsHuman)
                {
                    mAccessoryCategories.Add(BodyTypes.Glasses);
                    mAccessoryCategories.Add(BodyTypes.Earrings);
                    mAccessoryCategories.Add(BodyTypes.NoseRing);
                    mAccessoryCategories.Add(BodyTypes.LeftEarring);
                    mAccessoryCategories.Add(BodyTypes.RightEarring);
                    mAccessoryCategories.Add(BodyTypes.Necklace);
                    mAccessoryCategories.Add(BodyTypes.Armband);
                    mAccessoryCategories.Add(BodyTypes.Bracelet);
                    mAccessoryCategories.Add(BodyTypes.Gloves);
                    mAccessoryCategories.Add(BodyTypes.Ring);
                    if (mModel.CurrentOccultType != OccultTypes.Mermaid || !mModel.HiddenInCAS)
                    {
                        mAccessoryCategories.Add(BodyTypes.Socks);
                    }
                    mAccessoryCategories.Add(BodyTypes.Dental);
                    mAccessoryCategories.Add(BodyTypes.LeftGarter);
                    mAccessoryCategories.Add(BodyTypes.RightGarter);
                    mAccessoryCategories.Add(BodyTypes.BirthMark);
                }
                else if (mModel.Species == CASAgeGenderFlags.Horse || mModel.Species == CASAgeGenderFlags.Dog || mModel.Species == CASAgeGenderFlags.LittleDog || mModel.Species == CASAgeGenderFlags.Cat)
                {
                    mAccessoryCategories.Add(BodyTypes.PetBreastCollar);
                    mAccessoryCategories.Add(BodyTypes.PetBlanket);
                    mAccessoryCategories.Add(BodyTypes.PetBody);
                }
                mContentTypeFilter = CASPuck.GetContentTypeFilter();
                if (mContentTypeFilter != null)
                {
                    mContentTypeFilter.Tag = mSortButton;
                    WindowBase childByID = GetChildByID(98291488, true);
                    CASPuck.SetContentFilterPosition(childByID);
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

        // CASMAKEUP STUFF - MASCARA, HEADS, SCALPS
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
                CASFacialDetails casfacialDetails = CASFacialDetails.gSingleton;
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
                        casfacialDetails.SetLongPanel(true);
                        Tick -= casMakeup.OnTick;
                        casMakeup.mButtonCostumeFilter.Selected = false;
                        casMakeup.mContentTypeFilter.Visible = false;
                        break;
                    case BodyTypes.CostumeMakeup:
                        casMakeup.mWindowCostume.Visible = true;
                        casfacialDetails.SetLongPanel(false);
                        casfacialDetails.SetShortPanelHeight(570);
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
                    Button button = GetChildByID((uint)id, true) as Button;
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
                    casMakeup.mFadeEffect = effect as FadeEffect;
                    if (casMakeup.mFadeEffect != null)
                    {
                        casMakeup.mFadeTime = casMakeup.mFadeEffect.Duration;
                        break;
                    }
                }
                FadeTransitionFinished += casMakeup.OnFadeFinished;
                Init();
                CASMakeup.sCategory = BodyTypes.EyeShadow;
                Button button = GetChildByID(256, true) as Button;
                button.Selected = CASMakeup.sCategory == BodyTypes.EyeShadow;
                button.Click += OnButtonTabClick;
                button = GetChildByID(257, true) as Button;
                button.Selected = CASMakeup.sCategory == BodyTypes.EyeLiner;
                button.Click += OnButtonTabClick;

                // Mascara
                button = GetChildByID(262, true) as Button;
                button.Selected = CASMakeup.sCategory == BodyTypes.Mascara;
                button.Click += OnButtonTabClick;

                button = GetChildByID(258, true) as Button;
                button.Selected = CASMakeup.sCategory == BodyTypes.Blush;
                button.Click += OnButtonTabClick;
                button = GetChildByID(259, true) as Button;
                button.Selected = CASMakeup.sCategory == BodyTypes.FirstFace;
                button.Click += OnButtonTabClick;

                // Heads
                button = GetChildByID(263, true) as Button;
                button.Selected = CASMakeup.sCategory == BodyTypes.Face;
                button.Click += OnButtonTabClick;

                // Scalps
                button = GetChildByID(264, true) as Button;
                button.Selected = CASMakeup.sCategory == BodyTypes.Scalp;
                button.Click += OnButtonTabClick;

                button = GetChildByID(260, true) as Button;
                button.Selected = CASMakeup.sCategory == BodyTypes.CostumeMakeup;
                button.Click += OnButtonTabClick;

                button = GetChildByID(215017952, true) as Button;
                button.Selected = Responder.Instance.CASModel.PropagateMakeUpStyles;
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
                casMakeup.mWindowMakeup = GetChildByID(4096, true) as Window;
                casMakeup.mGridMakeupParts = casMakeup.mWindowMakeup.GetChildByID(4097, true) as ItemGrid;
                casMakeup.mGridMakeupPresets = casMakeup.mWindowMakeup.GetChildByID(4098, true) as ItemGrid;
                casMakeup.mButtonFilter = casMakeup.mWindowMakeup.GetChildByID(4102, true) as Button;
                casMakeup.mButtonFilter.Click += casMakeup.OnButtonFilterClick;
                casMakeup.mWindowCostume = GetChildByID(8192, true) as Window;
                casMakeup.mGridCostumeParts = casMakeup.mWindowCostume.GetChildByID(8193, true) as ItemGrid;
                casMakeup.mButtonCostumeFilter = casMakeup.mWindowCostume.GetChildByID(8198, true) as Button;
                casMakeup.mButtonCostumeFilter.Click += casMakeup.OnButtonCostumeFilterClick;
                casMakeup.mOpacitySlider = GetChildByID(158168880, true) as Slider;
                casMakeup.mOpacitySlider.MouseUp += casMakeup.OnOpacitySliderMouseUp;
                casMakeup.mOpacitySlider.SliderValueChange += casMakeup.OnOpacitySliderChange;
                casMakeup.mGridMakeupParts.ItemClicked += casMakeup.OnGridPartsClick;
                casMakeup.mGridMakeupPresets.ItemClicked += casMakeup.OnGridPresetsClick;
                casMakeup.mContentTypeFilter = CASPuck.GetContentTypeFilter();
                if (casMakeup.mContentTypeFilter != null)
                {
                    casMakeup.mContentTypeFilter.Tag = casMakeup.mButtonCostumeFilter;
                    WindowBase childByID = GetChildByID(8199, true);
                    CASPuck.SetContentFilterPosition(childByID);
                    casMakeup.mContentTypeFilter.FiltersChanged += casMakeup.OnFilterChanged;
                }
                casMakeup.mButtonColor = casMakeup.mWindowMakeup.GetChildByID(4101, true) as Button;
                casMakeup.mButtonColor.Click += casMakeup.OnButtonDesignClick;
                casMakeup.mButtonShare = casMakeup.mWindowMakeup.GetChildByID(4100, true) as Button;
                casMakeup.mButtonShare.Click += casMakeup.OnButtonShareClick;
                casMakeup.mButtonDelete = casMakeup.mWindowMakeup.GetChildByID(4099, true) as Button;
                casMakeup.mButtonDelete.Click += casMakeup.OnButtonDeleteClick;
                casMakeup.mButtonDeleteCostume = GetChildByID(8195, true) as Button;
                casMakeup.mButtonDeleteCostume.Click += casMakeup.OnButtonDeleteClick;
                casMakeup.mButtonShareCostume = GetChildByID(8196, true) as Button;
                casMakeup.mButtonShareCostume.Click += casMakeup.OnButtonShareClick;
                casMakeup.mButtonDesignCostume = GetChildByID(8197, true) as Button;
                casMakeup.mButtonDesignCostume.Click += casMakeup.OnButtonDesignClick;
                ICASModel cASModel = Responder.Instance.CASModel;
                cASModel.OnCASPartAdded += casMakeup.OnPartAdded;
                cASModel.OnCASPartRemoved += casMakeup.OnPartRemoved;
                cASModel.UndoSelected += casMakeup.OnUndoRedo;
                cASModel.RedoSelected += casMakeup.OnUndoRedo;
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
                    default:
                        return;
                }
            }
        }
    }
}
