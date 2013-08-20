APP_NAME=OpenStreetMap

# test call app
all: app test

# app

app: $(APP_NAME)/$(APP_NAME).pde

$(APP_NAME)/$(APP_NAME).pde: app.md $(APP_NAME).pde.mdlp
	mdlp -o ./ app.md $(APP_NAME).pde.mdlp Helper.pde.mdlp GraphTree.pde.mdlp

$(APP_NAME)/Helper.pde: app.md Helper.pde.mdlp
	mdlp -o ./ app.md $(APP_NAME).pde.mdlp Helper.pde.mdlp GraphTree.pde.mdlp

$(APP_NAME)/GraphTree.pde: app.md GraphTree.pde.mdlp
	mdlp -o ./ app.md $(APP_NAME).pde.mdlp Helper.pde.mdlp GraphTree.pde.mdlp

# test

test: test/Helper.pde test/GraphTree.pde

test/Helper.pde: $(APP_NAME)/Helper.pde
	cp $(APP_NAME)/Helper.pde test/Helper.pde

test/GraphTree.pde: $(APP_NAME)/GraphTree.pde
	cp $(APP_NAME)/GraphTree.pde test/GraphTree.pde

distclean:
	rm -rf $(APP_NAME)
	rm -f test/Helper.pde test/GraphTree.pde
