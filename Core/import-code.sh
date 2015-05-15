#!/bin/bash

# Import from Microsoft CoreFX
false && (
	git clone --depth 10 git@github.com:dotnet/corefx.git ../temp/corefx/
	cd ../temp/corefx/
	git pull
	git submodule update --init --recursive

	T=../../Core/Core.Portable/Cryptography/

	#cp src/System.Security.Cryptography.Hashing.Algorithms/src/System/Security/Cryptography/SHA1.cs $T
	#cp src/System.Security.Cryptography.Hashing.Algorithms/src/Internal/Cryptography/{HashAlgorithmNames,Helpers}.cs $T
)

# Import from Mono
(
	git clone --depth 10 git@github.com:mono/mono.git ../temp/mono
	cd ../temp/mono/
	git pull
	git submodule update --init --recursive

	T=../../Core/Core.Portable/Referencesource/

	cp external/referencesource/mscorlib/system/security/cryptography/{sha1managed,rijndael,rijndaelmanaged,rijndaelmanagedtransform,hashalgorithm,icryptotransform,symmetricalgorithm}.cs $T
)

for T in Core.Portable/Referencesource/
do
	sed -i '1s/^/using System;\nusing Core.Cryptography;\n/' $T/*
	sed -i \
		-e 's@using Internal.Cryptography;@@gm' \
		-e 's@using System.Security.Cryptography;@@gm' \
		-e 's@\[Serializable\]@@gm' \
		-e 's@namespace [ a-zA-Z0-9.]*@namespace Core.Referencesource@gm' \
		$T/*
done
