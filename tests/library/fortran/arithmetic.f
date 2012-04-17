      PROGRAM TEST
      INTEGER X
      
      X = 14 + 2
      ASSERT X.EQ.16
      
      X = 14 - 2
      ASSERT X.EQ.12
      
      X = 14 * 2
      ASSERT X.EQ.28
      
      X = 14 / 2
      ASSERT X.EQ.7
      
      X = 2 * 3 + 4
      ASSERT X.EQ.10
      
      X = 2 + 3 * 4
      ASSERT X.EQ.14
      
      X = 2 * (3 + 4)
      ASSERT X.EQ.14
      
      END
