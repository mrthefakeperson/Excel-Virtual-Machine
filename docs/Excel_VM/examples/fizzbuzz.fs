for e in 1..20 do
  if e % 15 = 0 then printfn "%s" "fizzbuzz"
  else
    if e % 3 = 0 then printfn "%s" "fizz"
    else
      if e % 5 = 0 then printfn "%s" "buzz"
      else printfn "%i" e