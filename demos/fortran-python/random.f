      PROGRAM PRINTRANDOM
      
      INTEGER N
      INTEGER SEED
      REAL RANDX
      
      SEED = 128
      RANDX = 0
      
      N = 0
 10   CALL RANDOM(SEED, RANDX)
      PRINT *, RANDX
      N = N + 1
      IF (N.LT.5) GOTO 10
      
      END

* --------------------------------------------
* Psuedo random number generator
* --------------------------------------------

      SUBROUTINE RANDOM(SEED, RANDX)

      INTEGER SEED
      REAL RANDX
      
      SEED = 2045*SEED + 1
      SEED = SEED - (SEED/1048576)*1048576
      RANDX = REAL(SEED + 1)/1048577.0
      RETURN

      END

* --------------------------------------------
