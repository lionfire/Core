﻿

//// -------------------------------------------------------------------------------
//// 
//// This file is part of the FluidKit project: http://www.codeplex.com/fluidkit
//// 
//// Copyright (c) 2008, The FluidKit community 
//// All rights reserved.
//// 
//// Redistribution and use in source and binary forms, with or without modification, 
//// are permitted provided that the following conditions are met:
//// 
//// * Redistributions of source code must retain the above copyright notice, this 
//// list of conditions and the following disclaimer.
//// 
//// * Redistributions in binary form must reproduce the above copyright notice, this 
//// list of conditions and the following disclaimer in the documentation and/or 
//// other materials provided with the distribution.
//// 
//// * Neither the name of FluidKit nor the names of its contributors may be used to 
//// endorse or promote products derived from this software without specific prior 
//// written permission.
//// 
//// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND 
//// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
//// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
//// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR 
//// ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES 
//// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; 
//// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON 
//// ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT 
//// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS 
//// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE. 
//// -------------------------------------------------------------------------------
//using System;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Media;
//using System.Windows.Media.Animation;

//namespace LionFire.Avalon.Transitions
//{
//    public abstract class Transition
//    {
//        private static ResourceDictionary _resources;
//        private Duration _duration = new Duration(TimeSpan.FromSeconds(1));
//        public TransitionPresenter Owner { get; internal set; }

//        static Transition()
//        {
//            _resources =
//                (ResourceDictionary)
//                Application.LoadComponent(
//                    new Uri("/FluidKit;;;component/Controls/Transition/TransitionStoryboards.xaml",
//                            UriKind.Relative));
//        }

//        protected static ResourceDictionary TransitionResources
//        {
//            get { return _resources; }
//            set { _resources = value; }
//        }

//        public Duration Duration
//        {
//            get { return _duration; }
//            set { _duration = value; }
//        }

//        protected NameScope GetNameScope()
//        {
//            NameScope scope = new NameScope();
//            NameScope.SetNameScope(Owner.TransitionContainer, scope);
//            return scope;
//        }

//        public abstract void Setup(Brush prevBrush, Brush nextBrush);

//        public abstract Storyboard PrepareStoryboard();
//        public virtual void Cleanup() { }
//    }
//}