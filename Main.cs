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
    // Classes with the [Plugin] attribute will be automatically created as soon as possible by MonoPatcher. You don't need a tuning XML to instantiate.
    [Plugin]
    public class Main 
    {
        public Main()
        {
            // Applies all patch attributes found in your mod's DLL. This should always be done in your plugin constructor, before most code is compiled.
            MonoPatcher.PatchAll();
        }
    
        // CASMAKEUP STUFF - MASCARA, HEADS, SCALPS
        [TypePatch(typeof(CASMakeup))]
        public class IHopeIExplode : CASFacialBlendPanel
        {
            public IHopeIExplode(uint winHandle) : base(winHandle)
            {
            }

            public override void Init()
            {
                CASMakeup swag = this as object as CASMakeup;

                foreach (object effect in base.EffectList)
                {
                    swag.mFadeEffect = effect as FadeEffect;
                    if (swag.mFadeEffect != null)
                    {
                        swag.mFadeTime = swag.mFadeEffect.Duration;
                        break;
                    }
                }
                base.FadeTransitionFinished += swag.OnFadeFinished;
                base.Init();
                CASMakeup.sCategory = BodyTypes.EyeShadow;
                Button button = GetChildByID(256u, recursive: true) as Button;
                button.Selected = CASMakeup.sCategory == BodyTypes.EyeShadow;
                button.Click += OnButtonTabClick;
                button = GetChildByID(257u, recursive: true) as Button;
                button.Selected = CASMakeup.sCategory == BodyTypes.EyeLiner;
                button.Click += OnButtonTabClick;
                
                //mascara
                button = GetChildByID(262u, recursive: true) as Button;
                button.Selected = CASMakeup.sCategory == BodyTypes.Mascara;
                button.Click += OnButtonTabClick;
                //mascara

                button = GetChildByID(258u, recursive: true) as Button;
                button.Selected = CASMakeup.sCategory == BodyTypes.Blush;
                button.Click += OnButtonTabClick;
                button = GetChildByID(259u, recursive: true) as Button;
                button.Selected = CASMakeup.sCategory == BodyTypes.FirstFace;
                button.Click += OnButtonTabClick;

                //heads
                button = GetChildByID(263u, recursive: true) as Button;
                button.Selected = CASMakeup.sCategory == BodyTypes.Face;
                button.Click += OnButtonTabClick;
                //heads

                //scalp
                button = GetChildByID(264u, recursive: true) as Button;
                button.Selected = CASMakeup.sCategory == BodyTypes.Scalp;
                button.Click += OnButtonTabClick;
                //scalp

                button = GetChildByID(260u, recursive: true) as Button;
                button.Selected = CASMakeup.sCategory == BodyTypes.CostumeMakeup;
                button.Click += OnButtonTabClick;

                button = GetChildByID(215017952u, recursive: true) as Button;
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
                button.Click += swag.OnLockCategoryButtonClick;
                swag.mWindowMakeup = GetChildByID(4096u, recursive: true) as Window;
                swag.mGridMakeupParts = swag.mWindowMakeup.GetChildByID(4097u, recursive: true) as ItemGrid;
                swag.mGridMakeupPresets = swag.mWindowMakeup.GetChildByID(4098u, recursive: true) as ItemGrid;
                swag.mButtonFilter = swag.mWindowMakeup.GetChildByID(4102u, recursive: true) as Button;
                swag.mButtonFilter.Click += swag.OnButtonFilterClick;
                swag.mWindowCostume = GetChildByID(8192u, recursive: true) as Window;
                swag.mGridCostumeParts = swag.mWindowCostume.GetChildByID(8193u, recursive: true) as ItemGrid;
                swag.mButtonCostumeFilter = swag.mWindowCostume.GetChildByID(8198u, recursive: true) as Button;
                swag.mButtonCostumeFilter.Click += swag.OnButtonCostumeFilterClick;
                swag.mOpacitySlider = GetChildByID(158168880u, recursive: true) as Slider;
                swag.mOpacitySlider.MouseUp += swag.OnOpacitySliderMouseUp;
                swag.mOpacitySlider.SliderValueChange += swag.OnOpacitySliderChange;
                swag.mGridMakeupParts.ItemClicked += swag.OnGridPartsClick;
                swag.mGridMakeupPresets.ItemClicked += swag.OnGridPresetsClick;
                swag.mContentTypeFilter = CASPuck.GetContentTypeFilter();
                if (swag.mContentTypeFilter != null)
                {
                    swag.mContentTypeFilter.Tag = swag.mButtonCostumeFilter;
                    WindowBase childByID = GetChildByID(8199u, recursive: true);
                    CASPuck.SetContentFilterPosition(childByID);
                    swag.mContentTypeFilter.FiltersChanged += swag.OnFilterChanged;
                }
                swag.mButtonColor = swag.mWindowMakeup.GetChildByID(4101u, recursive: true) as Button;
                swag.mButtonColor.Click += swag.OnButtonDesignClick;
                swag.mButtonShare = swag.mWindowMakeup.GetChildByID(4100u, recursive: true) as Button;
                swag.mButtonShare.Click += swag.OnButtonShareClick;
                swag.mButtonDelete = swag.mWindowMakeup.GetChildByID(4099u, recursive: true) as Button;
                swag.mButtonDelete.Click += swag.OnButtonDeleteClick;
                swag.mButtonDeleteCostume = GetChildByID(8195u, recursive: true) as Button;
                swag.mButtonDeleteCostume.Click += swag.OnButtonDeleteClick;
                swag.mButtonShareCostume = GetChildByID(8196u, recursive: true) as Button;
                swag.mButtonShareCostume.Click += swag.OnButtonShareClick;
                swag.mButtonDesignCostume = GetChildByID(8197u, recursive: true) as Button;
                swag.mButtonDesignCostume.Click += swag.OnButtonDesignClick;
                ICASModel cASModel = Responder.Instance.CASModel;
                cASModel.OnCASPartAdded += swag.OnPartAdded;
                cASModel.OnCASPartRemoved += swag.OnPartRemoved;
                cASModel.UndoSelected += swag.OnUndoRedo;
                cASModel.RedoSelected += swag.OnUndoRedo;
                SetCategory(CASMakeup.sCategory);
            }

            public new enum ControlIDs : uint
            {
                // Token: 0x0400305E RID: 12382
                ItemThumbnailWindow = 32U,
                // Token: 0x0400305F RID: 12383
                ItemCustom = 35U,
                // Token: 0x04003060 RID: 12384
                ItemWardrobe = 41U,
                // Token: 0x04003061 RID: 12385
                ItemThumbnailColor1 = 48U,
                // Token: 0x04003062 RID: 12386
                ItemThumbnailColor2,
                // Token: 0x04003063 RID: 12387
                ItemThumbnailColor3,
                // Token: 0x04003064 RID: 12388
                ItemThumbnailColor4,
                // Token: 0x04003065 RID: 12389
                ButtonEyeshadow = 256U,
                // Token: 0x04003066 RID: 12390
                ButtonEyeliner,
                // Token: 0x04003067 RID: 12391
                ButtonBlush,
                // Token: 0x04003068 RID: 12392
                ButtonLipstick,

                ButtonMascara = 262U,
                // Token: 0x04003069 RID: 12393
                ButtonCostume,
                // Token: 0x0400306A RID: 12394
                WindowMakeup = 4096U,
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
                WindowCostume = 8192U,
                // Token: 0x04003072 RID: 12402
                GridCostumeParts,
                // Token: 0x04003073 RID: 12403
                ButtonDeleteCostume = 8195U,
                // Token: 0x04003074 RID: 12404
                ButtonShareCostume,
                // Token: 0x04003075 RID: 12405
                ButtonDesignCostume,
                // Token: 0x04003076 RID: 12406
                ButtonCostumeFilter,
                // Token: 0x04003077 RID: 12407
                ContentTypeFilterPositionHolder,
                // Token: 0x04003078 RID: 12408
                LockCategoryButton = 215017952U,
                // Token: 0x04003079 RID: 12409
                SliderOpacity = 158168880U
            }

            public void HideUnusedIcons()
            {
                BodyTypes[] array = new BodyTypes[]
                    {
                        BodyTypes.EyeShadow,
                        BodyTypes.EyeLiner,
                        BodyTypes.Blush,
                        BodyTypes.Mascara,
                        BodyTypes.FirstFace,
                        BodyTypes.CostumeMakeup
                    };
                List<Button> list = new List<Button>();
                ICASModel casmodel = Responder.Instance.CASModel;
                foreach (BodyTypes bodyType in array)
                {
                    ArrayList visibleCASParts = casmodel.GetVisibleCASParts(bodyType);
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
                            id = (CASMakeup.ControlIDs)262u;
                            break;
                        case BodyTypes.EyeLiner:
                            id = CASMakeup.ControlIDs.ButtonEyeliner;
                            break;
                        case BodyTypes.Blush:
                            id = CASMakeup.ControlIDs.ButtonBlush;
                            break;
                    }
                    Button button = base.GetChildByID((uint)id, true) as Button;
                    if (visibleCASParts.Count > 0)
                    {
                        button.Visible = true;
                        list.Add(button);
                    }
                    else
                    {
                        button.Visible = false;
                        if (button.Selected)
                        {
                            button.Selected = false;
                            Button button2 = base.GetChildByID(260U, true) as Button;
                            button2.Selected = true;
                            if (CASMakeup.sCategory != BodyTypes.CostumeMakeup)
                            {
                                this.SetCategory(BodyTypes.CostumeMakeup);
                            }
                            return;
                        }
                    }
                }
                float num = (float)list.Count;
                float num2 = 110f - num * 0.5f * 42f - (num - 1f) * 0.5f * -3f;
                float num3 = num2;
                foreach (Button button3 in list)
                {
                    button3.Position = new Vector2(num3, button3.Position.y);
                    num3 += 39f;
                }
            }

            public void OnButtonTabClick(WindowBase sender, UIButtonClickEventArgs args)
            {
                switch (sender.ID)
                {
                    case 256U:
                        this.SetCategory(BodyTypes.EyeShadow);
                        return;
                    case 257U:
                        this.SetCategory(BodyTypes.EyeLiner);
                        return;
                    case 258U:
                        this.SetCategory(BodyTypes.Blush);
                        return;
                    case 259U:
                        this.SetCategory(BodyTypes.FirstFace);
                        return;
                    case 260U:
                        this.SetCategory(BodyTypes.CostumeMakeup);
                        return;
                    case 262U:
                        this.SetCategory(BodyTypes.Mascara);
                        return;
                    case 263U:
                        this.SetCategory(BodyTypes.Face);
                        return;
                    case 264U:
                        this.SetCategory(BodyTypes.Scalp);
                        return;
                    default:
                        return;
                }
            }

            private void SetCategory(BodyTypes category)
            {
                CASMakeup swag = this as object as CASMakeup;

                swag.mWindowCostume.Visible = false;
                swag.mWindowMakeup.Visible = false;
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
                        swag.mWindowMakeup.Visible = true;
                        casfacialDetails.SetLongPanel(true);
                        base.Tick -= swag.OnTick;
                        swag.mButtonCostumeFilter.Selected = false;
                        swag.mContentTypeFilter.Visible = false;
                        break;
                    case BodyTypes.CostumeMakeup:
                        swag.mWindowCostume.Visible = true;
                        casfacialDetails.SetLongPanel(false);
                        casfacialDetails.SetShortPanelHeight(570f);
                        if (swag.GetWornPart(category).Key != swag.kInvalidCASPart.Key)
                        {
                            swag.mButtonDesignCostume.Enabled = true;
                        }
                        else
                        {
                            swag.mButtonDesignCostume.Enabled = false;
                        }
                        swag.UpdateCostumePresetState();
                        base.Tick -= swag.OnTick;
                        base.Tick += swag.OnTick;
                        break;
                }
                CASMakeup.sCategory = category;
                this.HideUnusedIcons();
                swag.PopulatePartsGrid(CASMakeup.sCategory);

                swag.PopulatePresetsGrid(CASMakeup.sCategory, swag.GetWornPart(CASMakeup.sCategory), swag.mButtonFilter.Selected);
            }
        }

        //CASCLOTHINGCATEGORY - NOSERINGS, PET BODIES
        [TypePatch(typeof(CASClothingCategory))]
        public class WeAreSoBack : CASClothingCategory
        {
            public WeAreSoBack(uint winHandle)
        : base(winHandle)
            {
                mModel = Responder.Instance.CASModel;
            }

            public override void Init()
            {
                mAgeGenderFlags = mModel.Species | mModel.Age | mModel.Gender;
                foreach (object effect in base.EffectList)
                {
                    mFadeEffect = effect as FadeEffect;
                    if (mFadeEffect != null)
                    {
                        mFadeTime = mFadeEffect.Duration;
                        break;
                    }
                }
                mFading = true;
                base.FadeTransitionFinished += OnFadeFinished;
                mInvalidCASPart.Key = ResourceKey.kInvalidResourceKey;
                mTrashButton = GetChildByID(98285829u, recursive: true) as Button;
                mSaveButton = GetChildByID(98285828u, recursive: true) as Button;
                mShareButton = GetChildByID(98291479u, recursive: true) as Button;
                mSortButton = GetChildByID(98291481u, recursive: true) as Button;
                mCategoryText = GetChildByID(98291457u, recursive: true) as Text;
                mDesignButton = GetChildByID(98291482u, recursive: true) as Button;
                mShareButton.Enabled = false;
                mTrashButton.Enabled = false;
                mSaveButton.Enabled = false;
                mClothingTypesGrid = GetChildByID(98291489u, recursive: true) as ItemGrid;
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
                Button button = GetChildByID(98291478u, recursive: true) as Button;
                button.Click += OnButtonRandomizeClick;
                mCategoryButtonHolder = GetChildByID(98291472u, recursive: true) as Window;
                mTopsButton = mCategoryButtonHolder.GetChildByID(98291473u, recursive: true) as Button;
                mTopsButton.Click += OnCategoryButtonClick;
                mBottomsButton = mCategoryButtonHolder.GetChildByID(98291474u, recursive: true) as Button;
                mBottomsButton.Click += OnCategoryButtonClick;
                mShoesButton = mCategoryButtonHolder.GetChildByID(98291475u, recursive: true) as Button;
                mShoesButton.Click += OnCategoryButtonClick;
                mOutfitsButton = mCategoryButtonHolder.GetChildByID(98291476u, recursive: true) as Button;
                mOutfitsButton.Click += OnCategoryButtonClick;
                mAccessoriesButton = mCategoryButtonHolder.GetChildByID(98291600u, recursive: true) as Button;
                mAccessoriesButton.Click += OnCategoryButtonClick;
                mHorseBridlesButton = mCategoryButtonHolder.GetChildByID(98291483u, recursive: true) as Button;
                mHorseBridlesButton.Click += OnCategoryButtonClick;
                mHorseSaddleButton = mCategoryButtonHolder.GetChildByID(98291484u, recursive: true) as Button;
                mHorseSaddleButton.Click += OnCategoryButtonClick;
                mPartsList = new ArrayList();
                mPartPresetsList = new ArrayList();
                mbIsHuman = mModel.Species == CASAgeGenderFlags.Human || mModel.Species == CASAgeGenderFlags.None;
                if (mbIsHuman)
                {
                    MultiDrawable multiDrawable = mAccessoriesButton.Drawable as MultiDrawable;
                    IconDrawable iconDrawable = multiDrawable[1u] as IconDrawable;
                    iconDrawable.Image = UIManager.LoadUIImage(ResourceKey.CreatePNGKey("cas_clothing_i_acc_r2", 0u));
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
                    WindowBase childByID = GetChildByID(98291488u, recursive: true);
                    CASPuck.SetContentFilterPosition(childByID);
                    mContentTypeFilter.FiltersChanged += PopulateTypesGrid;
                }
                mModel.UndoSelected += OnUndoRedo;
                mModel.RedoSelected += OnUndoRedo;
                mModel.OnCASPartAdded += OnCASPartAddedRemoved;
                mModel.OnCASPartRemoved += OnCASPartAddedRemoved;
                Responder.Instance.StoreUI.OnFeaturedItemRemoveError += OnFeaturedItemRemoveErrorHandler;
                base.Tick += OnTick;
            }
        }

        [TypePatch(typeof(CAPTackCollarSheet))]
        public class DogyTime : CAPTackCollarSheet
        {

            public DogyTime(uint winHandle)
        : base(winHandle)
            {
            }

            public override void Init()
            {
                this.mTopState = ((Responder.Instance.CASModel.CASMode == CASMode.Tack) ? CASTopState.Tack : CASTopState.Collar);
                CASAgeGenderFlags species = Responder.Instance.CASModel.Species;
                WindowBase childByID;
                if (species != CASAgeGenderFlags.Horse)
                {
                    if (species != CASAgeGenderFlags.Cat)
                    {
                        childByID = base.GetChildByID(88908578U, true);
                    }
                    else
                    {
                        childByID = base.GetChildByID(88908577U, true);
                    }
                }
                else
                {
                    childByID = base.GetChildByID(88908576U, true);
                }
                childByID.Visible = true;
                for (uint num = 0U; num < 2U; num += 1U)
                {
                    this.mButtons[(int)((UIntPtr)num)] = (childByID.GetChildByID(88908321U + num, true) as Button);
                    this.mButtons[(int)((UIntPtr)num)].Click += this.OnNavigationButtonClick;
                    this.mButtons[(int)((UIntPtr)num)].FocusAcquired += this.OnNavigationFocusAcquired;
                    this.mButtons[(int)((UIntPtr)num)].MouseDown += this.OnNavigationButtonMouseDown;
                    this.mButtons[(int)((UIntPtr)num)].FocusLost += this.OnNavigationFocusLost;
                    this.mButtons[(int)((UIntPtr)num)].Tag = this.mButtons[(int)((UIntPtr)num)].TooltipText;
                    this.mButtons[(int)((UIntPtr)num)].TooltipText = "";
                    this.mButtonText[(int)((UIntPtr)num)] = (base.GetChildByID(88908305U + num, true) as Text);
                }
                this.mBaseColor = this.mButtonText[0].TextColor.ARGB;
                this.mTextWindow = (base.GetChildByID(88908304U, true) as Window);
                this.mTextWindow.EffectFinished += this.OnTextFadeFinished;
                foreach (object obj in this.mTextWindow.EffectList)
                {
                    this.mFadeEffect = (obj as FadeEffect);
                    if (this.mFadeEffect != null)
                    {
                        this.mFadeTime = this.mFadeEffect.Duration;
                        break;
                    }
                }
                this.mInputBlocker = base.GetChildByID(88908290U, true);
                this.mInputBlocker.Visible = false;
                Window window = base.GetChildByID(88908289U, true) as Window;
                window.Visible = true;
                window.EffectFinished += this.OnPanelGlideFinished;
                foreach (object obj2 in window.EffectList)
                {
                    this.mGlideEffect = (obj2 as GlideEffect);
                    if (this.mGlideEffect != null)
                    {
                        this.mGlideTime = this.mGlideEffect.Duration;
                        break;
                    }
                }
                base.FadeTransitionFinished += this.OnFadeFinished;
            }
        }
    }
}
