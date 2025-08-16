using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using Object = UnityEngine.Object;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace AnimatorAsCode.V1
{
    public class AacFlModification
    {
        private readonly AacConfiguration _configuration;
        private readonly AacFlBase _base;
        
        private readonly HashSet<Object> _objects = new();

        internal AacFlModification(AacConfiguration configuration, AacFlBase originalBase)
        {
            _configuration = configuration;
            _base = originalBase;
        }
        
        /// Immediately removes all layers and all parameters from the given AnimatorController, and returns a AacFlController that will edit the given AnimatorController.<br/>
        /// This AnimatorController instance is memorized in the current AacFlModification instance memory.<br/>
        /// Note: The AnimatorController class is editor-only, so they can't be referenced inside scene components or asset objects. If you have a RuntimeAnimatorController instance, you should cast it to AnimatorController.
        public AacFlController ResetAnimatorController(AnimatorController controllerToReset)
        {
            _objects.Add(controllerToReset);
            
            Internal_ClearAnimatorController(controllerToReset);

            return new AacFlController(_configuration, controllerToReset, _base);
        }

        /// Immediately removes all curves on the clip, and returns a AacFlClip that will edit the given AnimationClip.<br/>
        /// This does not reset any other attribute of the clip (e.g., is looping, etc.).<br/>
        /// This AnimationClip instance is memorized in the current AacFlModification instance memory.
        public AacFlClip ResetClip(AnimationClip clipToReset)
        {
            _objects.Add(clipToReset);
            
            clipToReset.ClearCurves();

            return new AacFlClip(_configuration, clipToReset);
        }

        /// Immediately clears the list of children in the given BlendTree, sets the parameters to empty strings, and returns a AacFlNonInitializedBlendTree that will edit the given BlendTree.<br/>
        /// This does not reset any other attribute of the blend tree (e.g., automatic thresholds, etc.).<br/>
        /// This BlendTree instance is memorized in the current AacFlModification instance memory.<br/>
        /// Note: The BlendTree class is editor-only, so they can't be referenced inside scene components or asset objects. If you have a Motion instance that is a BlendTree instance, you should cast it to BlendTree.
        public AacFlNonInitializedBlendTree ResetBlendTree(BlendTree blendTreeToReset)
        {
            _objects.Add(blendTreeToReset);
            
            blendTreeToReset.children = Array.Empty<ChildMotion>();
            blendTreeToReset.blendParameter = "";
            blendTreeToReset.blendParameterY = "";

            return new AacFlNonInitializedBlendTree(blendTreeToReset);
        }

        /// Immediately removes all layers and all parameters from the given AnimatorController.<br/>
        /// This AnimatorController instance is memorized in the current AacFlModification instance memory.<br/>
        /// Note: The AnimatorController class is editor-only, so they can't be referenced inside scene components or asset objects. If you have a RuntimeAnimatorController instance, you should cast it to AnimatorController.
        public AacFlModification ClearAnimatorController(AnimatorController controllerToReset)
        {
            _objects.Add(controllerToReset);
            
            Internal_ClearAnimatorController(controllerToReset);

            return this;
        }

        /// Returns a AacFlController that will edit the given AnimatorController. This does not reset the AnimatorController.<br/>
        /// This AnimatorController instance is memorized in the current AacFlModification instance memory.<br/>
        /// Note: The AnimatorController class is editor-only, so they can't be referenced inside scene components or asset objects. If you have a RuntimeAnimatorController instance, you should cast it to AnimatorController.
        public AacFlController EditAnimatorController(AnimatorController controllerToReset)
        {
            _objects.Add(controllerToReset);
            
            return new AacFlController(_configuration, controllerToReset, _base);
        }

        /// Calls `EditorUtility.SetDirty(...)` on every single AnimatorController, AnimationClip, and BlendTree asset instances previously memorized by this AacFlModification instance.
        public void SetDirtyAll()
        {
            foreach (var obj in _objects)
            {
                EditorUtility.SetDirty(obj);
            }
        }

        private static void Internal_ClearAnimatorController(AnimatorController controllerToReset)
        {
            while (controllerToReset.layers.Length > 0)
            {
                controllerToReset.RemoveLayer(0);
            }
            
            var parameters = controllerToReset.parameters;
            for (var i = parameters.Length - 1; i >= 0; i--)
            {
                controllerToReset.RemoveParameter(i);
            }
        }
    }
}