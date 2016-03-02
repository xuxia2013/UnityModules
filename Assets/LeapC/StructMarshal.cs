﻿using UnityEngine;
using System;
using System.Runtime.InteropServices;

namespace LeapInternal {

  /**
   * A helper class to marshal from unmanaged memory into structs without creating garbage.
   */
  public class StructMarshal<T> where T : struct {
    [StructLayout(LayoutKind.Sequential)]
    private class StructContainer {
      public T value;
    }

    private static StructContainer _container;
    private static int _sizeofT;

    private static GCHandle _tempHandle;
    private static IntPtr _tempPtr;

    static StructMarshal() {
      _container = new StructContainer();
      _sizeofT = Marshal.SizeOf(typeof(T));

      _tempHandle = GCHandle.Alloc(_container, GCHandleType.Pinned);
      _tempPtr = _tempHandle.AddrOfPinnedObject();
    }

    public static IntPtr StructToTempPtr(T t) {
      _container.value = t;
      Marshal.StructureToPtr(_container, _tempPtr, false);
      return _tempPtr;
    }

    /**
     * Converts an IntPtr to a struct of type T.  Does not allocate any
     * garbage unlike Marshal.PtrToStruct(IntPtr, Type).
     */
    public static T PtrToStruct(IntPtr ptr) {
      try {
        Marshal.PtrToStructure(ptr, _container);
        return _container.value;
      } catch (Exception e) {
        Logger.Log("Problem converting structure " + typeof(T) + " from ptr " + ptr + " : " + e.Message);
        return new T();
      }
    }

    /**
     * Converts a single element in an array pointed to by ptr to a struct
     * of type T.  This method does not and cannot do any bounds checking!
     * This method does not create any garbage.
     */
    public static T ArrayElementToStruct(IntPtr ptr, int arrayIndex) {
      return PtrToStruct(new IntPtr(ptr.ToInt64() + _sizeofT * arrayIndex));
    }
  }
}
