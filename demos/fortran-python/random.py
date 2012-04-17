def random(seed):
    seed = 2045*seed + 1
    seed = seed - (seed/1048576)*1048576
    randx = (seed + 1)/1048577.0
    return (seed, randx)

seed = 128

for n in range(5):
    (seed, randx) = random(seed)
    print randx
