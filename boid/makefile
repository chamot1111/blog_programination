UNITY_SCRIPT=BoidUnity/Assets/Scripts
#DEBUG_SYMBOL=-c //
DEBUG_SYMBOL=

all: app

# app

app: $(UNITY_SCRIPT)/Boid.cs $(UNITY_SCRIPT)/FlockManager.cs

$(UNITY_SCRIPT)/Boid.cs: boid.md Boid.cs.mdlp
	mdlp -o $(DEBUG_SYMBOL) $(UNITY_SCRIPT)/ boid.md Boid.cs.mdlp FlockManager.cs.mdlp

$(UNITY_SCRIPT)/FlockManager.cs: boid.md FlockManager.cs.mdlp
	mdlp -o $(DEBUG_SYMBOL) $(UNITY_SCRIPT)/ boid.md Boid.cs.mdlp FlockManager.cs.mdlp

# clean

distclean:
	rm -f $(UNITY_SCRIPT)/Boid.cs
	rm -f $(UNITY_SCRIPT)/FlockManager.cs
