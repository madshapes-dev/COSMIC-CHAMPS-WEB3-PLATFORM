using System;
using Cysharp.Threading.Tasks;
using ThirdParty.Extensions.Components;
using UniRx;
using UnityEngine;
using Zenject;

namespace CosmicChamps.UI
{
    public abstract class AbstractPresenter : MonoBehaviour
    {
        [SerializeField]
        private string _header;

        [SerializeField]
        private AbstractPresenter _backPresenter;

        [SerializeField]
        private bool _clearOnHide;

        [Inject]
        private IPresenterActivator _activator;

        protected IPresenterActivator Activator => _activator;
        protected bool ClearOnHide => _clearOnHide;

        protected readonly ObservableEvent<PresenterVisibilityEventArgs> _onDisplaying = new();
        protected readonly ObservableEvent<PresenterVisibilityEventArgs> _onHiding = new();

        public IObservable<PresenterVisibilityEventArgs> OnDisplaying => _onDisplaying.AsObservable ();
        public IObservable<PresenterVisibilityEventArgs> OnHiding => _onHiding.AsObservable ();

        public virtual string Header => _header;
        public virtual AbstractPresenter BackPresenter => _backPresenter;

        public abstract void ForceRefresh ();
        public abstract void ForceClear ();

        protected virtual void Awake ()
        {
        }

        protected virtual void OnDestroy ()
        {
            ForceClear ();
        }

        public virtual void ClosedWithBack ()
        {
        }

        public void Display (PresenterDisplayOptions options = PresenterDisplayOptions.Default)
        {
            async UniTaskVoid PerformDisplay () => await DisplayAsync (options);
            PerformDisplay ().Forget ();
        }

        public virtual async UniTask DisplayAsync (PresenterDisplayOptions options = PresenterDisplayOptions.Default)
        {
            Debug.Log ($"DisplayAsync {this}");
            if (options.HasFlag (PresenterDisplayOptions.Notify))
                _onDisplaying.Fire (new PresenterVisibilityEventArgs (this, options));

            await Activator.Activate (this, options.HasFlag (PresenterDisplayOptions.Immediate));
            ForceRefresh ();
        }

        public void Hide (PresenterDisplayOptions options = PresenterDisplayOptions.Default)
        {
            async UniTaskVoid PerformHide () => await HideAsync (options);
            PerformHide ().Forget ();
        }

        public virtual async UniTask HideAsync (PresenterDisplayOptions options = PresenterDisplayOptions.Default)
        {
            if (options.HasFlag (PresenterDisplayOptions.Notify))
                _onHiding.Fire (new PresenterVisibilityEventArgs (this, options));

            await Activator.Deactivate (this, options.HasFlag (PresenterDisplayOptions.Immediate));
            if (_clearOnHide)
                ForceClear ();
        }
    }

    public abstract class AbstractPresenter<TModel, TCallbacks, TSelf> : AbstractPresenter
        where TSelf : AbstractPresenter<TModel, TCallbacks, TSelf>
    {
        private TModel _model;

        protected readonly CompositeDisposable _callbacksDisposables = new();
        protected readonly CompositeDisposable _modelDisposables = new();

        public TModel model
        {
            get => _model;
            set
            {
                Clear ();

                _model = value;
                Refresh ();
            }
        }

        protected override void OnDestroy ()
        {
            _modelDisposables.Dispose ();
            _callbacksDisposables.Dispose ();
        }

        protected virtual void Clear ()
        {
            _modelDisposables.Clear ();
        }

        protected virtual void Refresh ()
        {
        }

        public void SetModelWithoutRefresh (TModel model) => _model = model;

        public sealed override void ForceRefresh ()
        {
            Clear ();
            Refresh ();
        }

        public sealed override void ForceClear ()
        {
            Clear ();
        }

        public void Display (
            TModel model,
            PresenterDisplayOptions options = PresenterDisplayOptions.Default)
        {
            async UniTaskVoid PerformDisplay () => await DisplayAsync (model, options);
            PerformDisplay ().Forget ();
        }

        public virtual async UniTask DisplayAsync (
            TModel model,
            PresenterDisplayOptions options = PresenterDisplayOptions.Default)
        {
            if (options.HasFlag (PresenterDisplayOptions.Notify))
                _onDisplaying.Fire (new PresenterVisibilityEventArgs (this, options));

            async UniTask SetModel ()
            {
                await UniTask.Yield ();
                this.model = model;
            }

            await UniTask.WhenAll (
                SetModel (),
                Activator.Activate (this, options.HasFlag (PresenterDisplayOptions.Immediate)));
        }

        public virtual void SetCallbacks (TCallbacks callbacks) => _callbacksDisposables.Clear ();
    }
}