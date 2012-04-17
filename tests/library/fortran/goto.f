      PROGRAM TEST
      
      INTEGER X
      
      X = 0
  10  X = X + 1
      IF (X.NE.100) GOTO 10
      
      ASSERT X.EQ.100
      
      END
