// Copyright (c) 2024 Francesco Crimi francrim@gmail.com
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using System;

namespace CiccioSoft.VirtualList.Sample.Wpf.Domain;

public class Model
{
    public Model(uint numero, string name)
    {
        Numero = numero;
        Name = name;
    }

    public virtual uint Id
    {
        get; set;
    }
    public virtual uint Numero
    {
        get; set;
    }
    public virtual string? Name
    {
        get; set;
    }

    public override bool Equals(object? obj)
    {
        return obj is Model model &&
               Numero == model.Numero;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Numero);
    }
}
