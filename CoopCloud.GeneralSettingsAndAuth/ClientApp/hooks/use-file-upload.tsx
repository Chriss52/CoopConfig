import type { FileMetadata } from "@/types";

import type React from "react";
import { formatBytes } from "@nubeteck/shadcn";
import {
  useRef,
  useState,
  useCallback,
  type DragEvent,
  type ChangeEvent,
  type InputHTMLAttributes,
} from "react";

export type FileUploadOptions = {
  maxFiles?: number; // Only used when multiple is true, defaults to Infinity
  maxSize?: number; // in bytes
  accept?: string;
  multiple?: boolean; // Defaults to false
  disabled?: boolean; // Defaults to false
  onFilesProcessed?: (files: File[]) => void; // Callback cuando se procesan archivos
  onError?: (errors: string[]) => void;
};

export type FileUploadState = {
  isDragging: boolean;
  errors: string[];
};

export type FileUploadActions = {
  processFiles: (files: File[] | FileList) => void;
  clearErrors: () => void;
  handleDragEnter: (e: DragEvent<HTMLElement>) => void;
  handleDragLeave: (e: DragEvent<HTMLElement>) => void;
  handleDragOver: (e: DragEvent<HTMLElement>) => void;
  handleDrop: (e: DragEvent<HTMLElement>) => void;
  handleFileChange: (e: ChangeEvent<HTMLInputElement>) => void;
  openFileDialog: () => void;
  getInputProps: (
    props?: InputHTMLAttributes<HTMLInputElement>,
  ) => InputHTMLAttributes<HTMLInputElement> & {
    ref: React.Ref<HTMLInputElement>;
  };
};

const useFileUpload = (
  options: FileUploadOptions = {},
): [FileUploadState, FileUploadActions] => {
  const {
    onError,
    accept = "*",
    disabled = false,
    multiple = false,
    onFilesProcessed,
    maxSize = Number.POSITIVE_INFINITY,
    maxFiles = Number.POSITIVE_INFINITY,
  } = options;

  const [state, setState] = useState<FileUploadState>({
    errors: [],
    isDragging: false,
  });

  const inputRef = useRef<HTMLInputElement>(null);

  const validateFile = useCallback(
    (file: File | FileMetadata): null | string => {
      // Validar tamaño del archivo
      if (file.size > maxSize) {
        return `El archivo "${file.name}" excede el tamaño máximo de ${formatBytes(maxSize)}.`;
      }

      // Validar tipo de archivo
      if (accept !== "*") {
        const acceptedTypes = accept.split(",").map((type) => type.trim());
        const fileType = file instanceof File ? file.type || "" : file.type;
        const fileExtension = `.${file.name.split(".").pop()}`;

        const isAccepted = acceptedTypes.some((type) => {
          if (type.startsWith(".")) {
            return fileExtension.toLowerCase() === type.toLowerCase();
          }
          if (type.endsWith("/*")) {
            const baseType = type.split("/")[0];
            return fileType.startsWith(`${baseType}/`);
          }
          return fileType === type;
        });

        if (!isAccepted) {
          return `El archivo "${file.name}" no es un tipo de archivo aceptado.`;
        }
      }

      return null;
    },
    [accept, maxSize],
  );

  // Función helper para validar límites de archivos
  const validateFileLimits = useCallback(
    (newFilesLength: number): null | string => {
      if (
        multiple &&
        maxFiles !== Number.POSITIVE_INFINITY &&
        newFilesLength > maxFiles
      ) {
        return `Solo puedes subir un máximo de ${maxFiles} archivos.`;
      }
      return null;
    },
    [multiple, maxFiles],
  );

  const processFileItem = useCallback(
    (file: File): { error: null | string; file: File } => {
      const error = validateFile(file);
      if (error) {
        return { error, file: {} as File };
      }
      return { file, error: null };
    },
    [validateFile],
  );

  const processFiles = useCallback(
    (newFiles: File[] | FileList) => {
      if (disabled || !newFiles || newFiles.length === 0) return;

      const newFilesArray = Array.from(newFiles);
      const errors: string[] = [];

      // Validar límites de archivos
      const limitError = validateFileLimits(newFilesArray.length);
      if (limitError) {
        errors.push(limitError);
        onError?.(errors);
        setState((prev) => ({ ...prev, errors }));
        return;
      }

      const validFiles: File[] = [];

      // Procesar cada archivo
      for (const file of newFilesArray) {
        const { error, file: processedFile } = processFileItem(file);
        if (error) {
          errors.push(error);
        } else {
          validFiles.push(processedFile);
        }
      }

      // Actualizar estado con errores (si los hay)
      setState((prev) => ({ ...prev, errors }));

      // Llamar callback con archivos procesados
      if (validFiles.length > 0) {
        onFilesProcessed?.(validFiles);
      }

      // Reportar errores si los hay
      if (errors.length > 0) {
        onError?.(errors);
      }

      // Resetear input
      if (inputRef.current) {
        inputRef.current.value = "";
      }
    },
    [disabled, validateFileLimits, processFileItem, onFilesProcessed, onError],
  );

  const clearErrors = useCallback(() => {
    setState((prev) => ({
      ...prev,
      errors: [],
    }));
  }, []);

  const handleDragEnter = useCallback(
    (e: DragEvent<HTMLElement>) => {
      e.preventDefault();
      e.stopPropagation();
      if (disabled) return;
      setState((prev) => ({ ...prev, isDragging: true }));
    },
    [disabled],
  );

  const handleDragLeave = useCallback(
    (e: DragEvent<HTMLElement>) => {
      e.preventDefault();
      e.stopPropagation();

      if (disabled || e.currentTarget.contains(e.relatedTarget as Node)) {
        return;
      }

      setState((prev) => ({ ...prev, isDragging: false }));
    },
    [disabled],
  );

  const handleDragOver = useCallback(
    (e: DragEvent<HTMLElement>) => {
      e.preventDefault();
      e.stopPropagation();
      if (disabled) return;
    },
    [disabled],
  );

  const handleDrop = useCallback(
    (e: DragEvent<HTMLElement>) => {
      e.preventDefault();
      e.stopPropagation();
      setState((prev) => ({ ...prev, isDragging: false }));

      // Don't process files if disabled or input is disabled
      if (disabled || inputRef.current?.disabled) {
        return;
      }

      if (e.dataTransfer.files && e.dataTransfer.files.length > 0) {
        // In single file mode, only use the first file
        if (!multiple) {
          const file = e.dataTransfer.files[0];
          processFiles([file]);
        } else {
          processFiles(e.dataTransfer.files);
        }
      }
    },
    [disabled, processFiles, multiple],
  );

  const handleFileChange = useCallback(
    (e: ChangeEvent<HTMLInputElement>) => {
      if (disabled || !e.target.files || e.target.files.length === 0) return;
      processFiles(e.target.files);
    },
    [disabled, processFiles],
  );

  const openFileDialog = useCallback(() => {
    if (disabled || !inputRef.current) return;
    inputRef.current.click();
  }, [disabled]);

  const getInputProps = useCallback(
    (props: InputHTMLAttributes<HTMLInputElement> = {}) => {
      return {
        ...props,
        ref: inputRef,
        type: "file" as const,
        onChange: handleFileChange,
        accept: props.accept || accept,
        disabled: props.disabled !== undefined ? props.disabled : disabled,
        multiple: props.multiple !== undefined ? props.multiple : multiple,
      };
    },
    [accept, disabled, multiple, handleFileChange],
  );

  return [
    state,
    {
      handleDrop,
      clearErrors,
      processFiles,
      getInputProps,
      handleDragOver,
      openFileDialog,
      handleDragEnter,
      handleDragLeave,
      handleFileChange,
    },
  ];
};

export default useFileUpload;
