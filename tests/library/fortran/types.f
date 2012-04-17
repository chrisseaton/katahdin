      PROGRAM TEST
      INTEGER X
      REAL Y
      
      X = 14.1
      ASSERT X.EQ.14
      
      Y = 14
      X = Y / 3
      ASSERT X.EQ.4
      
      X = 14
      Y = X / 3.0
      ASSERT Y.LT.4.67
      ASSERT Y.GT.4.65
      
      Y = X / REAL(3)
      ASSERT Y.LT.4.67
      ASSERT Y.GT.4.65
      
      END
