      PROGRAM TEST
      INTEGER X
      X = 2
      CALL SUB(X)
      ASSERT X.EQ.14
      END

      SUBROUTINE SUB(X)
      INTEGER X
      ASSERT X.EQ.2
      X = 14
      RETURN
      END
