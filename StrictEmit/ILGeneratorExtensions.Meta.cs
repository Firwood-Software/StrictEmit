//
//  ILGeneratorExtensions.Meta.cs
//
//  Copyright (c) 2018 Jarl Gullberg
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
//

using System.Reflection;
using System.Reflection.Emit;
using JetBrains.Annotations;

namespace StrictEmit
{
    public static partial class ILGeneratorExtensions
    {
        /// <summary>
        /// Emits a set of IL instructions which will retrieve the current method, and get the argument specified by the
        /// given index, pushing it as a <see cref="ParameterInfo"/> onto the evaluation stack.
        /// </summary>
        /// <param name="il">The generator where the IL is to be emitted.</param>
        /// <param name="argumentIndex">The index of the argument to get.</param>
        [PublicAPI]
        public static void EmitGetCurrentMethodArgumentByIndex([NotNull] this ILGenerator il, int argumentIndex)
        {
            if (argumentIndex == 0)
            {
                il.EmitGetCurrentMethodReturnParameter();
            }
            else
            {
                il.EmitGetCurrentMethodParameterByIndex(argumentIndex - 1);
            }
        }

        /// <summary>
        /// Emits a set of IL instructions which will retrieve the current method, get its parameters, and then get the
        /// parameter at the given index, pushing it onto the evaluation stack.
        /// </summary>
        /// <param name="il">The generator where the IL is to be emitted.</param>
        /// <param name="parameterIndex">The index of the parameter to get in the parameter array.</param>
        [PublicAPI]
        public static void EmitGetCurrentMethodParameterByIndex([NotNull] this ILGenerator il, int parameterIndex)
        {
            il.EmitCallDirect<MethodBase>(nameof(MethodBase.GetCurrentMethod));
            il.EmitCallVirtual<MethodBase>(nameof(MethodBase.GetParameters));
            il.EmitConstantInt(parameterIndex);
            il.EmitLoadArrayElement(typeof(MethodBase));
        }

        /// <summary>
        /// Emits a set of IL instructions which will retrieve the current method, get its return value parameter, and
        /// push it onto the evaluation stack.
        /// </summary>
        /// <param name="il">The generator where the IL is to be emitted.</param>
        [PublicAPI]
        public static void EmitGetCurrentMethodReturnParameter([NotNull] this ILGenerator il)
        {
            // ReSharper disable once PossibleNullReferenceException
            var getReturnParamFunc = typeof(MethodInfo).GetProperty(nameof(MethodInfo.ReturnParameter), BindingFlags.Public | BindingFlags.Instance).GetMethod;

            il.EmitCallDirect<MethodBase>(nameof(MethodBase.GetCurrentMethod));
            il.EmitCallVirtual(getReturnParamFunc);
        }
    }
}
