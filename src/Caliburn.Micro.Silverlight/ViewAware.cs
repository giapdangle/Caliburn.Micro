﻿namespace Caliburn.Micro {
    using System;
    using System.Collections.Generic;
    using System.Windows;

    ///<summary>
    ///  A base implementation of <see cref = "IViewAware" /> which is capable of caching views by context.
    ///</summary>
    public class ViewAware : PropertyChangedBase, IViewAware {
        static readonly DependencyProperty PreviouslyAttachedProperty = DependencyProperty.RegisterAttached(
            "PreviouslyAttached",
            typeof(bool),
            typeof(ViewAware),
            null
            );

        bool cacheViews;

        /// <summary>
        ///   The view chache for this instance.
        /// </summary>
        protected readonly Dictionary<object, object> Views = new Dictionary<object, object>();

        ///<summary>
        /// Creates an instance of <see cref="ViewAware"/>.
        ///</summary>
        public ViewAware()
            : this(ConventionManager.CacheViewsByDefault) {}

        ///<summary>
        /// Creates an instance of <see cref="ViewAware"/>.
        ///</summary>
        ///<param name="cacheViews">Indicates whether or not this instance maintains a view cache.</param>
        public ViewAware(bool cacheViews) {
            CacheViews = cacheViews;
        }

        /// <summary>
        ///   Raised when a view is attached.
        /// </summary>
        public event EventHandler<ViewAttachedEventArgs> ViewAttached = delegate { };

        ///<summary>
        ///  Indicates whether or not this instance maintains a view cache.
        ///</summary>
        protected bool CacheViews {
            get { return cacheViews; }
            set {
                cacheViews = value;
                if(!cacheViews)
                    Views.Clear();
            }
        }

        /// <summary>
        ///   Attaches a view to this instance.
        /// </summary>
        /// <param name = "view">The view.</param>
        /// <param name = "context">The context in which the view appears.</param>
        public virtual void AttachView(object view, object context = null) {
            if (CacheViews)
                Views[context ?? View.DefaultContext] = view;

            var element = view as FrameworkElement;
            if (element != null && !(bool)element.GetValue(PreviouslyAttachedProperty)) {
                element.SetValue(PreviouslyAttachedProperty, true);
                View.ExecuteOnLoad(element, (s, e) => OnViewLoaded(s));
            }

            ViewAttached(this, new ViewAttachedEventArgs { View = view, Context = context });
        }

        /// <summary>
        ///   Called when an attached view's Loaded event fires.
        /// </summary>
        /// <param name = "view"></param>
        protected virtual void OnViewLoaded(object view) {}

        /// <summary>
        ///   Gets a view previously attached to this instance.
        /// </summary>
        /// <param name = "context">The context denoting which view to retrieve.</param>
        /// <returns>The view.</returns>
        public virtual object GetView(object context = null) {
            object view;
            Views.TryGetValue(context ?? View.DefaultContext, out view);
            return view;
        }
    }
}