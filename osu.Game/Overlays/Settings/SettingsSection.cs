﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osuTK.Graphics;

namespace osu.Game.Overlays.Settings
{
    public abstract class SettingsSection : Container, IHasFilterableChildren
    {
        protected FillFlowContainer FlowContent;
        protected override Container<Drawable> Content => FlowContent;

        private IBindable<SettingsSection> selectedSection;

        private OsuSpriteText header;

        public abstract Drawable CreateIcon();
        public abstract LocalisableString Header { get; }

        public IEnumerable<IFilterable> FilterableChildren => Children.OfType<IFilterable>();
        public virtual IEnumerable<string> FilterTerms => new[] { Header.ToString() };

        private const int header_size = 26;
        private const int margin = 20;
        private const int border_size = 2;

        public bool MatchingFilter
        {
            set => this.FadeTo(value ? 1 : 0);
        }

        public bool FilteringActive { get; set; }

        [Resolved]
        private SettingsPanel settingsPanel { get; set; }

        protected SettingsSection()
        {
            AutoSizeAxes = Axes.Y;
            RelativeSizeAxes = Axes.X;

            FlowContent = new FillFlowContainer
            {
                Margin = new MarginPadding
                {
                    Top = header_size
                },
                Direction = FillDirection.Vertical,
                AutoSizeAxes = Axes.Y,
                RelativeSizeAxes = Axes.X,
            };
        }

        [BackgroundDependencyLoader]
        private void load(OsuColour colours)
        {
            AddRangeInternal(new Drawable[]
            {
                new Box
                {
                    Name = "separator",
                    Colour = new Color4(0, 0, 0, 255),
                    RelativeSizeAxes = Axes.X,
                    Height = border_size,
                },
                new Container
                {
                    Padding = new MarginPadding
                    {
                        Top = margin + border_size,
                        Bottom = margin + 10,
                    },
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Children = new Drawable[]
                    {
                        header = new OsuSpriteText
                        {
                            Font = OsuFont.GetFont(size: header_size),
                            Text = Header,
                            Colour = colours.Yellow,
                            Margin = new MarginPadding
                            {
                                Left = SettingsPanel.CONTENT_MARGINS,
                                Right = SettingsPanel.CONTENT_MARGINS
                            }
                        },
                        FlowContent
                    }
                },
            });

            selectedSection = settingsPanel.CurrentSection.GetBoundCopy();
            selectedSection.BindValueChanged(_ => updateContentFade(), true);
        }

        private bool isCurrentSection => selectedSection.Value == this;

        protected override bool OnHover(HoverEvent e)
        {
            updateContentFade();
            return base.OnHover(e);
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            updateContentFade();
            base.OnHoverLost(e);
        }

        protected override bool OnClick(ClickEvent e)
        {
            if (!isCurrentSection)
                settingsPanel.SectionsContainer.ScrollTo(this);

            return base.OnClick(e);
        }

        protected override bool ShouldBeConsideredForInput(Drawable child) =>
            // only the current section should accept input.
            // this provides the behaviour of the first click scrolling the target section to the centre of the screen.
            isCurrentSection;

        private void updateContentFade()
        {
            float contentFade = 1;
            float headerFade = 1;

            if (!isCurrentSection)
            {
                contentFade = 0.25f;
                headerFade = IsHovered ? 0.5f : 0.25f;
            }

            header.FadeTo(headerFade, 500, Easing.OutQuint);
            FlowContent.FadeTo(contentFade, 500, Easing.OutQuint);
        }
    }
}
